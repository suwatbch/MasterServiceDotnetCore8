using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtaxService.Models
{
    [Table("Permission", Schema = "dbo")]
    public class Permission
    {
        [Key]
        public int ID { get; set; }
        
        [Column("name")]  
        public string? Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("roleId")]
        public string? RoleId { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    
    }
}