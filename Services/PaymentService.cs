using EtaxService.Models;
using EtaxService.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EtaxService.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<List<ICSUpdatePaymentLog>> GetICSUpdatePaymentLog()
        {
            return await _paymentRepository.GetICSUpdatePaymentLog();
        }

        public async Task<ICSUpdatePaymentLog> GetFirstPaymentLogAsync()
        {
            return await _paymentRepository.GetFirstPaymentLog();
        }

        public async Task<bool> UpdatePaymentLogAsync(string ref1, string invoiceNo)
        {
            return await _paymentRepository.UpdatePaymentLogAsync(ref1, invoiceNo);
        }
    }
}
