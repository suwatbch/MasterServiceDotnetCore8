namespace EtaxService.DTOs.Request
{   
    public class RoleToUserUpdateRequest{
        public int ID { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }
    }
    public class RoleToUserDeleteRequest{
        public int ID { get; set; }
    }
    public class RoleToUserCreateRequest{
        public int RoleId { get; set; }
        public int UserId { get; set; }
    }
    
}
