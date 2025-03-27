using EtaxService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EtaxService.Repositories
{
    public interface IPaymentRepository
    {
        Task<List<ICSUpdatePaymentLog>> GetICSUpdatePaymentLog();
        Task<ICSUpdatePaymentLog> GetFirstPaymentLog();
        Task<bool> UpdatePaymentLogAsync(string ref1, string invoiceNo);
    }
}
