using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtaxService.Models
{
    [Table("RoleToUser", Schema = "dbo")]
    public class RoleToUser
    {        
        [Key]
        public int? ID { get; set; }

        [Column("RoleId")]  
        public int? RoleId { get; set; }
    
        [Column("UserId")]
        public int? UserId { get; set; }

        public User User { get; set; }
        public Role Role { get; set; }
    }
}