using EtaxService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EtaxService.Services
{
    public interface IGenfileService
    {
        void GeneratePDFQuestPdf();
        void GeneratePDFsharpCore();
        void GeneratePDFItext7();
        void GeneratePDFIronPdf();
        void GeneratePDFSyncfusionPdf();
    }
}
