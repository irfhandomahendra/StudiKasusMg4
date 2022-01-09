using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Dtos;
using AuthServer.Models;

namespace AuthServer.Data
{
    public interface IUser
    {
        IEnumerable<string> GetAllUser();
        IEnumerable<string> GetRoles();
        Task AddRole(string rolename);
        Task AddUserToRole(UserRole userRole);
        Task<List<string>> GetRolesFromUser(string username);
        Task Registration(CreateUserDto user);
        Task<User> Authenticate(CreateUserDto userDto);
    }
}