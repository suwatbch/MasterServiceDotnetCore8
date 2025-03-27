using EtaxService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EtaxService.Services
{
    public interface IPaymentService
    {
        Task<List<ICSUpdatePaymentLog>> GetICSUpdatePaymentLog();
        Task<ICSUpdatePaymentLog> GetFirstPaymentLogAsync();
        Task<bool> UpdatePaymentLogAsync(string ref1, string invoiceNo);
    }
}
