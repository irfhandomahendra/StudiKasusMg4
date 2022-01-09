using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthServer.Dtos;
using AuthServer.Helpers;
using AuthServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer.Data
{
    public class UserDAL : IUser
    {
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IOptions<AppSettings> _appSettings;

        public UserDAL(UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _appSettings = appSettings;
        }
        public async Task AddRole(string rolename)
        {
            IdentityResult roleResult;
            try
            {
                var roleIsExist = await _roleManager.RoleExistsAsync(rolename);
                if (!roleIsExist)
                    roleResult = await _roleManager.CreateAsync(new IdentityRole(rolename));
                else
                    throw new System.Exception($"Role {rolename} sudah ada");
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }
        }

        public async Task AddUserToRole(UserRole userRole)
        {
            var user = await _userManager.FindByNameAsync(userRole.Username);
            try
            {
                await _userManager.AddToRoleAsync(user, userRole.Rolename);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception($"Error: {ex.Message}");
            }
        }

        public async Task<User> Authenticate(CreateUserDto userDto)
        {
            var userFind = await _userManager.CheckPasswordAsync(
                await _userManager.FindByNameAsync(userDto.Username), userDto.Password);
            if (!userFind)
                return null;

            var user = new User
            {
                Username = userDto.Username
            };

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Username));
            var roles = await GetRolesFromUser(userDto.Username);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Value.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            return user;
        }

        public IEnumerable<string> GetAllUser()
        {
            List<string> users = new List<string>();
            var results = _userManager.Users;
            foreach(var user in results)
            {
                users.Add(user.UserName);
            }
            return users;
        }

        public IEnumerable<string> GetRoles()
        {
            List<string> lstRole = new List<string>();
            var results = _roleManager.Roles;
            foreach(var role in results)
            {
                lstRole.Add(role.Name);
            }
            return lstRole;
        }

        public async Task<List<string>> GetRolesFromUser(string username)
        {
            List<string> lstRoles = new List<string>();
            var user = await _userManager.FindByEmailAsync(username);

            var roles = await _userManager.GetRolesAsync(user);
            foreach(var role in roles)
            {
                lstRoles.Add(role);
            }
            return lstRoles;
        }

        public async Task Registration(CreateUserDto user)
        {
            try
            {
                var newUser = new IdentityUser
                {
                    UserName = user.Username,Email = user.Username
                };
                var result = await _userManager.CreateAsync(newUser, user.Password);
                if (!result.Succeeded)
                    throw new System.Exception("Gagal menambahkan user");
            }
            catch (System.Exception ex)
            {
                throw new System.Exception($"Error: {ex.Message}");
            }
        }
    }
}