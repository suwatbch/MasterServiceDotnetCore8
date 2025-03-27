using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtaxService.Models
{
    [Table("User", Schema = "dbo")]
    public class User
    {
        [Key]
        public int ID { get; set; }
        
        [Column("username")]  
        public string? Username { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("role")]
        public string? Role { get; set; }

        [Column("name")]
        public string? NameDisplay { get; set; }

        [Column("hashedPassword")]
        public string? PasswordHash { get; set; }

        [Column("salt")]
        public string? PasswordSalt { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    
    }
}