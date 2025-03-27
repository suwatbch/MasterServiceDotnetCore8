using EtaxService.DTOs.Request;
using EtaxService.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EtaxService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenfileController : BaseController
    {
        private readonly IGenfileService _genfileService;
        private readonly IConfiguration _configuration;

        public GenfileController(IGenfileService genfileService, IConfiguration configuration)
        {
            _genfileService = genfileService ?? throw new ArgumentNullException(nameof(genfileService));
            _configuration = configuration;
        }

        [HttpGet("GetMessage")]
        public IActionResult GetMessage()
        {
            return Ok(new { message = "Hello World PDF" });
        }

        [HttpGet("GeneratePDFQuestPdf")]
        public IActionResult GeneratePDFQuestPdf()
        {
            try
            {
                _genfileService.GeneratePDFQuestPdf();
                return Ok(new { 
                    message = "เริ่มกระบวนการสร้าง PDF แล้ว", 
                    status = "processing" 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    error = $"เกิดข้อผิดพลาด: {ex.Message}" 
                });
            }
        }

        [HttpGet("GeneratePDFsharpCore")]
        public IActionResult GeneratePDFsharpCore()
        {
            try
            {
                _genfileService.GeneratePDFsharpCore();
                return Ok(new { 
                    message = "เริ่มกระบวนการสร้าง PDF แล้ว", 
                    status = "processing" 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    error = $"เกิดข้อผิดพลาด: {ex.Message}" 
                });
            }
        }

        [HttpGet("GeneratePDFItext7")]
        public IActionResult GeneratePDFItext7()
        {
            try
            {
                _genfileService.GeneratePDFItext7();
                return Ok(new { 
                    message = "เริ่มกระบวนการสร้าง PDF แล้ว", 
                    status = "processing" 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    error = $"เกิดข้อผิดพลาด: {ex.Message}"
                });
            }
        }

        [HttpGet("GeneratePDFIronPdf")]
        public IActionResult GeneratePDFIronPdf()
        {
            try
            {
                _genfileService.GeneratePDFIronPdf();
                return Ok(new { 
                    message = "เริ่มกระบวนการสร้าง PDF แล้ว", 
                    status = "processing" 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    error = $"เกิดข้อผิดพลาด: {ex.Message}"
                });
            }
        }

        [HttpGet("GeneratePDFSyncfusionPdf")]
        public IActionResult GeneratePDFSyncfusionPdf()
        {
            try
            {
                _genfileService.GeneratePDFSyncfusionPdf();
                return Ok(new { 
                    message = "เริ่มกระบวนการสร้าง PDF แล้ว", 
                    status = "processing" 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    error = $"เกิดข้อผิดพลาด: {ex.Message}"
                });
            }
        }
    }
}

    
