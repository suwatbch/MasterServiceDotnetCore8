using EtaxService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EtaxService.Services
{
    public interface IEtaxService
    {
        Task<List<User>> GetUser();
        Task<List<User>> GetUserById(int id);
        Task<List<User>> CreateUser(User user);
        Task<List<User>> UpdateUser(User user);
        Task<List<User>> DeleteUser(int id);

        Task<List<Role>> CreateRole(Role role);
        Task<List<Role>> UpdateRole(Role role);
        Task<List<Role>> DeleteRole(int id);
        Task<List<Role>> GetRoles();

        Task<List<Permission>> CreatePermission(Permission permission);
        Task<List<Permission>> UpdatePermission(Permission permission);
        Task<List<Permission>> DeletePermission(int id);
        Task<List<Permission>> GetPermission();

        Task<List<RoleToUser>> CreateUserRole(RoleToUser roleToUser);
        Task<List<RoleToUser>> UpdateUserRole(RoleToUser roleToUser);
        Task<List<RoleToUser>> DeleteUserRole(int id);
        Task<List<RoleToUser>> GetUserRole();
    }
}
