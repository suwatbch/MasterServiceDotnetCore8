using EtaxService.DTOs.Request;
using EtaxService.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EtaxService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("GetMessage")]
        public async Task<IActionResult> GetMessage()
        {
            return Ok(new { message = "Hello World" });
        }

        [HttpGet("GetICSUpdatePaymentLog")]
        public async Task<IActionResult> GetICSUpdatePaymentLog()
        {
            var logs = await _paymentService.GetICSUpdatePaymentLog();
            return Ok(logs);
        }

        [HttpGet("GetFirstPaymentLog")]
        public async Task<IActionResult> GetFirstPaymentLog()
        {
            var log = await _paymentService.GetFirstPaymentLogAsync();
            if (log == null)
                return NotFound(new { message = "No payment log found" });

            return Ok(log);
        }

        [HttpPost("UpdatePaymentLog")]
        public async Task<IActionResult> UpdatePaymentLog([FromBody] PaymentRequest request)
        {
            var result = await _paymentService.UpdatePaymentLogAsync(request.Ref1, request.InvoiceNo);
            if (!result)
                return NotFound(new { message = "Payment log not found" });

            return Ok(new { message = "Payment log updated successfully" });
        }

    }
}

    
