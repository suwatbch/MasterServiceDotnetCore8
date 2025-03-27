using EtaxService.Models;
using EtaxService.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EtaxService.Services
{
    public class EtaxService : IEtaxService
    {
        private readonly IEtaxRepository _etaxRepository;

        public EtaxService(IEtaxRepository etaxRepository)
        {
            _etaxRepository = etaxRepository;
        }

        #region User
        public async Task<List<User>> GetUser()
        {
            return await _etaxRepository.GetUser();
        }
        public async Task<List<User>> GetUserById(int id)
        {
            return await _etaxRepository.GetUserById(id);
        }
        public async Task<List<User>> CreateUser(User user)
        {
            return await _etaxRepository.CreateUser(user);
        }
        public async Task<List<User>> UpdateUser(User user)
        {
            return await _etaxRepository.UpdateUser(user);
        }

        public async Task<List<User>> DeleteUser(int id)
        {
            return await _etaxRepository.DeleteUser(id);
        }
        #endregion


        #region Role 
        public async Task<List<Role>> CreateRole(Role role)
        {
            return await _etaxRepository.CreateRole(role);
        }
        public async Task<List<Role>> UpdateRole(Role role)
        {
            return await _etaxRepository.UpdateRole(role);
        }
        public async Task<List<Role>> DeleteRole(int id)
        {
            return await _etaxRepository.DeleteRole(id);
        }
        public async Task<List<Role>> GetRoles()
        {
            return await _etaxRepository.GetRoles();
        }
        #endregion


        #region Permission 
        public async Task<List<Permission>> CreatePermission(Permission permission)
        {
            return await _etaxRepository.CreatePermission(permission);
        }
        public async Task<List<Permission>> UpdatePermission(Permission permission)
        {
            return await _etaxRepository.UpdatePermission(permission);
        }  
        public async Task<List<Permission>> DeletePermission(int id)
        {
            return await _etaxRepository.DeletePermission(id);
        }
        public async Task<List<Permission>> GetPermission()
        {
            return await _etaxRepository.GetPermission();
        }
        #endregion


        #region RoleToUser 
        public async Task<List<RoleToUser>> CreateUserRole(RoleToUser roleToUser)
        {
            return await _etaxRepository.CreateUserRole(roleToUser);
        }
        public async Task<List<RoleToUser>> UpdateUserRole(RoleToUser roleToUser)
        {
            return await _etaxRepository.UpdateUserRole(roleToUser);
        }
        public async Task<List<RoleToUser>> DeleteUserRole(int id)
        {
            return await _etaxRepository.DeleteUserRole(id);
        }
        public async Task<List<RoleToUser>> GetUserRole()
        {
            return await _etaxRepository.GetUserRole();
        }
        #endregion
   
    }
}
