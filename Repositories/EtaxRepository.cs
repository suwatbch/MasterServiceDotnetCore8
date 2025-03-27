using EtaxService.Configuration;
using EtaxService.Database;
using EtaxService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace EtaxService.Repositories
{
    public class EtaxRepository : IEtaxRepository
    {
        private readonly EtaxDatabaseContext _context;

        public EtaxRepository(EtaxDatabaseContext context)
        {
            _context = context;
        }

        #region User
        public async Task<List<User>> GetUser()
        {
            return await _context.Users.ToListAsync();
        }
        
        public async Task<List<User>> GetUserById(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ID == id);
            return user != null ? new List<User> { user } : new List<User>();
        }
        
        public async Task<List<User>> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return await _context.Users.ToListAsync();
        }
        
        public async Task<List<User>> UpdateUser(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Username);
            
            if (existingUser != null)
            {
                // Update specific properties
                existingUser.NameDisplay = user.NameDisplay;
                existingUser.Email = user.Email;
                existingUser.Role = user.Role;
                existingUser.UpdatedAt = DateTime.UtcNow;

                // Update other properties as needed
                
                await _context.SaveChangesAsync();
            }
            
            return await _context.Users.ToListAsync();
        }
                
        public async Task<List<User>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return await _context.Users.ToListAsync();
        }
        #endregion

        #region Role 
        public async Task<List<Role>> CreateRole(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return await _context.Roles.ToListAsync();
        }
        
        public async Task<List<Role>> UpdateRole(Role role)
        {
            var existingRole = await _context.Roles.FindAsync(role.ID);
            
            if (existingRole != null)
            {
                // Update specific properties
                existingRole.Name = role.Name;
                existingRole.Description = role.Description;
                existingRole.UpdatedAt = DateTime.UtcNow;

                // Update other properties as needed
                
                await _context.SaveChangesAsync();
            }
            
            return await _context.Roles.ToListAsync();
        }
        
        public async Task<List<Role>> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
            }
            return await _context.Roles.ToListAsync();
        }
        
        public async Task<List<Role>> GetRoles()
        {
            return await _context.Roles.ToListAsync();
        }
        #endregion

        #region Permission 
        public async Task<List<Permission>> GetPermission()
        {
            return await _context.Permissions.ToListAsync();
        }
        
        public async Task<List<Permission>> UpdatePermission(Permission permission)
        {
            var existingPermission = await _context.Permissions.FindAsync(permission.ID);
            
            if (existingPermission != null)
            {
                // Update specific properties
                existingPermission.Name = permission.Name;
                existingPermission.Description = permission.Description;
                existingPermission.RoleId = permission.RoleId;
                existingPermission.UpdatedAt = DateTime.UtcNow;

                // Update other properties as needed
                
                await _context.SaveChangesAsync();
            }
            
            return await _context.Permissions.ToListAsync();
        }
        
        public async Task<List<Permission>> DeletePermission(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission != null)
            {
                _context.Permissions.Remove(permission);
                await _context.SaveChangesAsync();
            }
            return await _context.Permissions.ToListAsync();
        }
        
        public async Task<List<Permission>> CreatePermission(Permission permission)
        {
            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();
            return await _context.Permissions.ToListAsync();
        }
        #endregion

        #region RoleToUser 
        public async Task<List<RoleToUser>> GetUserRole()
        {
            return await _context.RoleToUsers.ToListAsync();
        }
        
        public async Task<List<RoleToUser>> UpdateUserRole(RoleToUser roleToUser)
        {
            var existingRoleToUser = await _context.RoleToUsers.FindAsync(roleToUser.ID);
            
            if (existingRoleToUser != null)
            {
                // Update specific properties
                existingRoleToUser.RoleId = roleToUser.RoleId;
                existingRoleToUser.UserId = roleToUser.UserId;

                // Update other properties as needed
                
                await _context.SaveChangesAsync();
            }
            
            return await _context.RoleToUsers.ToListAsync();
        }
        
        public async Task<List<RoleToUser>> DeleteUserRole(int id)
        {
            var roleToUser = await _context.RoleToUsers.FindAsync(id);
            if (roleToUser != null)
            {
                _context.RoleToUsers.Remove(roleToUser);
                await _context.SaveChangesAsync();
            }
            return await _context.RoleToUsers.ToListAsync();
        }
        
        public async Task<List<RoleToUser>> CreateUserRole(RoleToUser roleToUser)
        {
            _context.RoleToUsers.Add(roleToUser);
            await _context.SaveChangesAsync();
            return await _context.RoleToUsers.ToListAsync();
        }
        #endregion
    }
}
