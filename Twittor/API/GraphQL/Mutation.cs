using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Dtos;
using API.Models;
using HotChocolate;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.GraphQL
{
    public class Mutation
    {
        public async Task<UserDto> RegisterUserAsync(
            RegisterUser input,
            [Service] AppDbContext context)
        {
            var user = context.Users.Where(o => o.Username == input.UserName).FirstOrDefault();
            if (user != null)
            {
                return await Task.FromResult(new UserDto());
            }
            var newUser = new User
            {
                FullName = input.FullName,
                Email = input.Email,
                Username = input.UserName,
                Password = BCrypt.Net.BCrypt.HashPassword(input.Password)
            };

            var ret = context.Users.Add(newUser);
            await context.SaveChangesAsync();

            return await Task.FromResult(new UserDto
            {
                Id = newUser.Id,
                Username = newUser.Username,
                Email = newUser.Email,
                FullName = newUser.FullName
            });
        }

        public async Task<RoleDto> AddRoleAsync(
            string roleName,
            [Service] AppDbContext context)
        {
            var role = context.Roles.Where(o => o.Name == roleName).FirstOrDefault();
            if (role != null)
            {
                return await Task.FromResult(new RoleDto());
            }
            var newRole = new Role
            {
                Name = roleName
            };

            var ret = context.Roles.Add(newRole);
            await context.SaveChangesAsync();

            return await Task.FromResult(new RoleDto
            {
                Id = newRole.Id,
                Name = newRole.Name
            });
        }

        public async Task<UserRoleDto> AddRoleToUserAsync(
            UserRoleInput input,
            [Service] AppDbContext context)
        {
            var userRole = context.UserRoles.Where(o => o.UserId == input.UserId && 
            o.RoleId == input.RoleId).FirstOrDefault();
            if (userRole != null)
            {
                return await Task.FromResult(new UserRoleDto());
            }

            var newUserRole = new UserRole
            {
                UserId = input.UserId,
                RoleId = input.RoleId
            };

            var ret = context.UserRoles.Add(newUserRole);
            await context.SaveChangesAsync();

            return await Task.FromResult(new UserRoleDto{
                UserId = newUserRole.UserId,
                RoleId = newUserRole.RoleId
            });
        }

        public async Task<UserToken> LoginAsync(
            LoginUser input,
            [Service] IOptions<TokenSettings> tokenSettings,
            [Service] AppDbContext context)
        {
            var user = context.Users.Where(o => o.Username == input.Username).FirstOrDefault();
            if (user == null)
            {
                return await Task.FromResult(new UserToken(null, null, "Username or password was invalid"));
            }
            bool valid = BCrypt.Net.BCrypt.Verify(input.Password, user.Password);
            if (valid)
            {
                var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Value.Key));
                var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, user.Username));

                foreach (var userRole in user.UserRoles)
                {
                    var role = context.Roles.Where(o => o.Id == userRole.RoleId).FirstOrDefault();
                    if (role != null)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.Name));
                    }
                }

                var expired = DateTime.Now.AddHours(3);
                var jwtToken = new JwtSecurityToken(
                    issuer: tokenSettings.Value.Issuer,
                    audience: tokenSettings.Value.Audience,
                    expires: expired,
                    claims: claims,
                    signingCredentials: credentials
                );

                return await Task.FromResult(
                    new UserToken(new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expired.ToString(), null));
                //return new JwtSecurityTokenHandler().WriteToken(jwtToken);
            }

            return await Task.FromResult(new UserToken(null, null, Message: "Username or password was invalid"));
        }

    }
}