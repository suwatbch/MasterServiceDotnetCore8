using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EtaxService.Models;

namespace EtaxService.Database
{
    public class EtaxDatabaseContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public EtaxDatabaseContext(DbContextOptions<EtaxDatabaseContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        // DbSet สำหรับ Etax เท่านั้น
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RoleToUser> RoleToUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", schema: "dbo");
       
                // Add any other columns that exist in your database table
            });
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role", schema: "dbo");
       
                // Add any other columns that exist in your database table
            });
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permission", schema: "dbo");
       
                // Add any other columns that exist in your database table
            });
            modelBuilder.Entity<RoleToUser>(entity =>
            {
                entity.ToTable("RoleToUser", schema: "dbo");
       
                // Add any other columns that exist in your database table
            });
        }
    }
} 