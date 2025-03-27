namespace EtaxService.DTOs.Request
{   
    public class UserUpdateRequest{
        public string Username { get; set; }
        public string Displayname { get; set; }
        public int Trsg { get; set; }
        public int Bp { get; set; }
        public string Expired { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string Action { get; set; }
 

    
    }
    public class UserDeleteRequest{
        public int ID { get; set; }
    }
    
}
