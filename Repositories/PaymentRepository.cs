using EtaxService.Configuration;
using EtaxService.Database;
using EtaxService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace EtaxService.Repositories
{
    public class PaymentRepository : GenericRepository<ICSUpdatePaymentLog>, IPaymentRepository
    {
        private readonly DefaultDatabaseContext _defaultContext;

        public PaymentRepository(DefaultDatabaseContext context) : base(context)
        {
            _defaultContext = context;
        }

        public async Task<List<ICSUpdatePaymentLog>> GetICSUpdatePaymentLog()
        {
            return await _defaultContext.ICSUpdatePaymentLogs.ToListAsync();
        }

        public async Task<ICSUpdatePaymentLog> GetFirstPaymentLog()
        {
            return await _defaultContext.ICSUpdatePaymentLogs
                .OrderBy(x => x.Ref1)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdatePaymentLogAsync(string ref1, string invoiceNo)
        {
            var log = await _defaultContext.ICSUpdatePaymentLogs.FirstOrDefaultAsync(x => x.Ref1 == ref1);

            if (log == null)
                return false;

            log.InvoiceNo = invoiceNo; 
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
