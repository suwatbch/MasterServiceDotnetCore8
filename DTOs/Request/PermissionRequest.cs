namespace EtaxService.DTOs.Request
{   
    public class PermissionUpdateRequest{
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RoleId { get; set; }
    }
    public class PermissionDeleteRequest{
        public int ID { get; set; }
    }
    public class PermissionCreateRequest{
        public string Name { get; set; }
        public string Description { get; set; }
        public string RoleId { get; set; }
    }
    
}
