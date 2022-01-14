using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Auth;
using API.Data;
using API.Dtos;
using API.Kafka;
using API.Models;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace API.GraphQL
{
    public class Mutation
    {
        public async Task<TransactionStatus> RegisterUserAsync(
            RegisterUser input,
            [Service] AppDbContext context,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var user = context.Users.Where(o => o.Username == input.UserName).FirstOrDefault();
            if (user != null)
            {
                return new TransactionStatus(false, "Username already exist");
            }
            var newUser = new User
            {
                Fullname = input.FullName,
                Email = input.Email,
                Username = input.UserName,
                Password = BCrypt.Net.BCrypt.HashPassword(input.Password)
            };
            var key = "User-Add-" + DateTime.Now.ToString();
            var val = JObject.FromObject(newUser).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "User", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to submit data");

            return await Task.FromResult(ret);
        }

        public async Task<TransactionStatus> AddRoleAsync(
            string roleName,
            [Service] AppDbContext context,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var role = context.Roles.Where(o => o.Name == roleName).FirstOrDefault();
            if (role != null)
            {
                return new TransactionStatus(false, "Role already exist");
            }
            var newRole = new Role
            {
                Name = roleName
            };

            var key = "Role-Add-" + DateTime.Now.ToString();
            var val = JObject.FromObject(newRole).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "Role", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to submit data");

            return await Task.FromResult(ret);
        }

        public async Task<TransactionStatus> AddRoleToUserAsync(
            UserRoleInput input,
            [Service] AppDbContext context,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var userRole = context.UserRoles.Where(o => o.UserId == input.UserId &&
            o.RoleId == input.RoleId).FirstOrDefault();
            if (userRole != null)
            {
                return new TransactionStatus(false, "Role already exist in this user");
            }

            var newUserRole = new UserRole
            {
                UserId = input.UserId,
                RoleId = input.RoleId
            };

            var key = "User-Role-Add-" + DateTime.Now.ToString();
            var val = JObject.FromObject(newUserRole).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "UserRole", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to submit data");

            return await Task.FromResult(ret);
        }

        public async Task<UserToken> LoginAsync(
            LoginUser input,
            [Service] IOptions<TokenSettings> tokenSettings,
            [Service] AppDbContext context,
            [Service] IOptions<KafkaSettings> kafkaSettings)
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

                var userRoles = context.UserRoles.Where(o => o.UserId == user.Id).ToList();

                foreach (var userRole in userRoles)
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

                var key = "SignIn-" + DateTime.Now.ToString();
                var val = JObject.FromObject(new { Message = $"{input.Username} has signed in" }).ToString(Formatting.None);
                await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);

                return await Task.FromResult(
                    new UserToken(new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expired.ToString(), null));
            }

            return await Task.FromResult(new UserToken(null, null, Message: "Username or password was invalid"));
        }

        [Authorize(Roles = new[] { "admin", "member" })]
        public async Task<TransactionStatus> EditProfilAsync(
            ProfileInput input,
            [Service] AppDbContext context,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var profile = context.Users.Where(o => o.Id == input.Id).FirstOrDefault();
            if (profile != null)
            {
                profile.Fullname = input.FullName;
                profile.Email = input.Email;
                profile.Username = input.Username;
                profile.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);

                var key = "Edit-Profile-" + DateTime.Now.ToString();
                var val = JObject.FromObject(profile).ToString(Formatting.None);
                var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "EditProfile", key, val);
                await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);

                var ret = new TransactionStatus(result, "");
                if (!result)
                    ret = new TransactionStatus(result, "Failed to submit data");
                return await Task.FromResult(ret);
            }
            else
            {
                return new TransactionStatus(false, "Profile doesn't exist");
            }
        }

        [Authorize(Roles = new[] { "admin", "member" })]
        public async Task<TransactionStatus> ChangePasswordAsync(
            ChangePasswordInput input,
            [Service] AppDbContext context,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var akun = context.Users.Where(o => o.Username == input.Username).FirstOrDefault();
            if (akun != null)
            {
                akun.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
                var key = "Change-Password-" + DateTime.Now.ToString();
                var val = JObject.FromObject(akun).ToString(Formatting.None);
                var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "ChangePassword", key, val);
                await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);

                var ret = new TransactionStatus(result, "");
                if (!result)
                    ret = new TransactionStatus(result, "Failed to submit data");
                return await Task.FromResult(ret);
            }
            else
            {
                return new TransactionStatus(false, "User doesn't exist");
            }
        }

        [Authorize(Roles = new[] { "admin" })]
        public async Task<TransactionStatus> ChangeUserRoleAsync(
            UserRoleInput input,
            [Service] AppDbContext context,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var userRole = context.UserRoles.Where(o => o.UserId == input.UserId).FirstOrDefault();
            if (userRole != null)
            {
                userRole.RoleId = input.RoleId;
                var key = "Change-Role-" + DateTime.Now.ToString();
                var val = JObject.FromObject(userRole).ToString(Formatting.None);
                var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "ChangeRole", key, val);
                await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);

                var ret = new TransactionStatus(result, "");
                if (!result)
                    ret = new TransactionStatus(result, "Failed to submit data");
                return await Task.FromResult(ret);
            };
            return new TransactionStatus(false, "User doesn't exist");
        }

        [Authorize(Roles = new[] { "admin" })]
        public async Task<TransactionStatus> LockUserAsync(
            int userId,
            [Service] AppDbContext context,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var userRoles = context.UserRoles.Where(o => o.UserId == userId).ToList();
            bool check = false;
            if (userRoles != null)
            {
                foreach (var userRole in userRoles)
                {
                    var key = "Lock-User-" + DateTime.Now.ToString();
                    var val = JObject.FromObject(userRole).ToString(Formatting.None);
                    var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "LockUser", key, val);
                    await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);
                    var ret = new TransactionStatus(result, "");
                    check = true;
                };

                if (!check)
                    return new TransactionStatus(false, "Failed to submit data");
                return await Task.FromResult(new TransactionStatus(true, ""));
            }
            else
            {
                return new TransactionStatus(false, "User doesnt have any role yet");
            }
        }

        [Authorize(Roles = new[] { "member" })]
        public async Task<TransactionStatus> AddTwittorAsync(
            TwittorInput input,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var twittor = new Twittor
            {
                UserId = input.UserId,
                Message = input.Message,
                Created = DateTime.Now
            };
            var key = "Twit-Add-" + DateTime.Now.ToString();
            var val = JObject.FromObject(twittor).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "Twit", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to submit data");
            return await Task.FromResult(ret);
        }

        [Authorize(Roles = new[] { "member" })]
        public async Task<TransactionStatus> AddCommentAsync(
            CommentInput input,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var comment = new Comment
            {
                TwittorId = input.TwittorId,
                Message = input.Message
            };
            var key = "Comment-Add-" + DateTime.Now.ToString();
            var val = JObject.FromObject(comment).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "Comment", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to submit data");
            return await Task.FromResult(ret);
        }

        [Authorize(Roles = new[] { "member" })]
        public async Task<TransactionStatus> DeleteTwittorAsync(
            int userId,
            [Service] AppDbContext context,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var twits = context.Twittors.Where(o => o.UserId == userId).ToList();
            bool check = false;
            if (twits != null)
            {
                foreach (var twit in twits)
                {
                    var key = "Delete-Twit-" + DateTime.Now.ToString();
                    var val = JObject.FromObject(twit).ToString(Formatting.None);
                    var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "DeleteTwit", key, val);
                    await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);
                    var ret = new TransactionStatus(result, "");
                    check = true;
                }

                if (!check)
                    return new TransactionStatus(false, "Failed to submit data");
                return await Task.FromResult(new TransactionStatus(true, ""));
            }
            else
            {
                return new TransactionStatus(false, "User has zero twit");
            }
        }

    }
}