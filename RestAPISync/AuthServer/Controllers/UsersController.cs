using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Data;
using AuthServer.Dtos;
using AuthServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUser _user;

        public UsersController(IUser user)
        {
            _user = user;
        }

        [HttpPost]
        public async Task<ActionResult> Registration(CreateUserDto user)
        {
            try
            {
                await _user.Registration(user);
                return Ok($"Registrasi user {user.Username} berhasil");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetAll()
        {
            return Ok(_user.GetAllUser());
        }

        [HttpPost("Role")]
        public async Task<ActionResult> AddRole(string rolename)
        {
            try
            {
                await _user.AddRole(rolename);
                return Ok($"Tambah role {rolename} berhasil");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Role")]
        public ActionResult<IEnumerable<string>> GetAllRole()
        {
            return Ok(_user.GetRoles());
        }

        [HttpPost("UserInRole")]
        public async Task<ActionResult> AddUserToRole(UserRole userRole)
        {
            try
            {
                await _user.AddUserToRole(userRole);
                return Ok($"Berhasil menambahkan user {userRole.Username} ke role {userRole.Rolename}");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("RolesByUser/{username}")]
        public async Task<ActionResult<List<string>>> GetRolesByUser(string username)
        {
            var results = await _user.GetRolesFromUser(username);
            return Ok(results);
        }

        [HttpPost("Authentication")]
        public async Task<ActionResult<User>> Authentication(CreateUserDto createUserDto)
        {
            try
            {
                var user = await _user.Authenticate(createUserDto);
                if (user == null)
                    return BadRequest("username/password tidak tepat");
                return Ok(user);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}