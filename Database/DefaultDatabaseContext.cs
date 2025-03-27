using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EtaxService.Models;

namespace EtaxService.Database
{
    public class DefaultDatabaseContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DefaultDatabaseContext(DbContextOptions<DefaultDatabaseContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<ICSUpdatePaymentLog> ICSUpdatePaymentLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ICSUpdatePaymentLog>().ToTable("ICSUpdatePaymentLog", schema: "ta");
        }
    }
} 