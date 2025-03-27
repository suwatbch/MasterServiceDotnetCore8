using EtaxService.Configuration;
using EtaxService.Database;
using EtaxService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace EtaxService.Repositories
{
    public class GenfileRepository : GenericRepository<string>, IGenfileRepository
    {
        public GenfileRepository(DefaultDatabaseContext context) : base(context)
        {
            // ไม่ต้องประกาศ _context ใหม่
        }
    }
}
