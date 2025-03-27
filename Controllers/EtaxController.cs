using EtaxService.DTOs.Request;
using EtaxService.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EtaxService.Database;
using Microsoft.Extensions.Configuration;
using EtaxService.Models;

namespace EtaxService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtaxController : BaseController
    {
        private readonly IEtaxService _etaxService;

        public EtaxController(IEtaxService etaxService)
        {
            _etaxService = etaxService;
        }
        //User CRUD
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            var users = await _etaxService.GetUser();
            return Ok(users);
        }
        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateRequest userRequest){
            var userModel = new User
            {
                // Map properties from userRequest to userModel
                Username = userRequest.Username,
                NameDisplay = userRequest.Displayname,
                Role = userRequest.Role,
                // Add other properties as needed
            };
            var user = await _etaxService.UpdateUser(userModel); 
            return Ok(user);
        }
        [HttpPost("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromBody] UserDeleteRequest userRequest){
            var user = await _etaxService.DeleteUser(userRequest.ID);
            return Ok(user);
        }

        //CRUD Role
        [HttpGet("GetRole")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _etaxService.GetRoles();
            return Ok(roles);
        }
        [HttpPost("UpdateRole")]
        public async Task<IActionResult> UpdateRole([FromBody] RoleUpdateRequest roleRequest){
            var roleModel = new Role{
                // Map properties from userRequest to userModel
                ID = roleRequest.ID,
                Name = roleRequest.Name,
                Description = roleRequest.Description,
                // Add other properties as needed
            };
            var role = await _etaxService.UpdateRole(roleModel);
            return Ok(role);
        }
        [HttpPost("DeleteRole")]
        public async Task<IActionResult> DeleteRole([FromBody] RoleDeleteRequest roleRequest){
            var role = await _etaxService.DeleteRole(roleRequest.ID);
            return Ok(role);
        }
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateRequest roleRequest){
            var roleModel = new Role{
                Name = roleRequest.Name,
                Description = roleRequest.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var role = await _etaxService.CreateRole(roleModel);
            return Ok(role);
        }

        //CRUD Permission
        [HttpGet("GetPermission")]
        public async Task<IActionResult> GetPermission()
        {
            var Permission = await _etaxService.GetPermission();
            return Ok(Permission);
        }
        [HttpPost("UpdatePermission")]
        public async Task<IActionResult> UpdatePermission([FromBody] PermissionUpdateRequest permissionRequest){
            var permissionModel = new Permission{
                // Map properties from userRequest to userModel
                ID = permissionRequest.ID,
                Name = permissionRequest.Name,
                Description = permissionRequest.Description,
                RoleId = permissionRequest.RoleId,
                // Add other properties as needed
            };
            var permission = await _etaxService.UpdatePermission(permissionModel);
            return Ok(permission);
        }
        [HttpPost("DeletePermission")]
        public async Task<IActionResult> DeletePermission([FromBody] PermissionDeleteRequest permissionRequest){
            var permission = await _etaxService.DeletePermission(permissionRequest.ID);
            return Ok(permission);
        }
        [HttpPost("CreatePermission")]
        public async Task<IActionResult> CreatePermission([FromBody] PermissionCreateRequest permissionRequest){
            var permissionModel = new Permission{
                Name = permissionRequest.Name,
                Description = permissionRequest.Description,
                RoleId = permissionRequest.RoleId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var permission = await _etaxService.CreatePermission(permissionModel);
            return Ok(permission);
        }

        //CRUD RoleToUser
        [HttpGet("GetRoleToUser")]
        public async Task<IActionResult> GetUserRole()
        {
            var RoleToUser = await _etaxService.GetUserRole();
            return Ok(RoleToUser);
        }
        [HttpPost("UpdateRoleToUser")]
        public async Task<IActionResult> UpdateUserRole([FromBody] RoleToUserUpdateRequest roleToUserRequest){
            var roleToUserModel = new RoleToUser{
                // Map properties from userRequest to userModel
                ID = roleToUserRequest.ID,
                RoleId = roleToUserRequest.RoleId,
                UserId = roleToUserRequest.UserId,
                // Add other properties as needed
            };
            var roleToUser = await _etaxService.UpdateUserRole(roleToUserModel);
            return Ok(roleToUser);
        }
        [HttpPost("DeleteRoleToUser")]
        public async Task<IActionResult> DeleteUserRole([FromBody] RoleToUserDeleteRequest roleToUserRequest){
            var roleToUser = await _etaxService.DeleteUserRole(roleToUserRequest.ID);
            return Ok(roleToUser);
        }
        [HttpPost("CreateRoleToUser")]
        public async Task<IActionResult> CreateUserRole([FromBody] RoleToUserCreateRequest roleToUserRequest){
            var roleToUserModel = new RoleToUser{
                RoleId = roleToUserRequest.RoleId,
                UserId = roleToUserRequest.UserId,
            };
            var roleToUser = await _etaxService.CreateUserRole(roleToUserModel);
            return Ok(roleToUser);
        }
    }
}

    
