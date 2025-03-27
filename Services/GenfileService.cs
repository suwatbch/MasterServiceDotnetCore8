using EtaxService.Models;
using EtaxService.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Fonts;
using iText.Kernel.Pdf;
using iText.Kernel.Font;
using iText.IO.Font;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Image;
using iText.Layout.Borders;
using IronPdf;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;

namespace EtaxService.Services
{
    public class GenfileService : IGenfileService
    {
        private readonly IGenfileRepository _genfileRepository;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public GenfileService(IGenfileRepository genfileRepository)
        {
            _genfileRepository = genfileRepository ?? throw new ArgumentNullException(nameof(genfileRepository));
        }

        // --------- Generate QuestPDF ---------

        public void GeneratePDFQuestPdf()
        {
            Task.Run(async () =>
            {
                try
                {
                    await _semaphore.WaitAsync();
                    var startTime = DateTime.Now; 
                    
                    try
                    {
                        int count = 1;
                        int batchSize = 100;
                        
                        for (int start = 0; start < count; start += batchSize)
                        {
                            var tasks = new List<Task>();
                            int end = Math.Min(start + batchSize, count);

                            for (int i = start; i < end; i++)
                            {
                                int index = i;
                                tasks.Add(Task.Run(() => 
                                {
                                    GenerateSinglePDFQuestPdf(index);
                                    Console.WriteLine($"Generated PDF {index + 1} of {count}");
                                }));
                            }

                            Task.WaitAll(tasks.ToArray());
                            Console.WriteLine($"Completed batch: {start + 1} to {end}");
                        }

                        Console.WriteLine($"Completed all PDFs. Total time: {DateTime.Now - startTime}");
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }

        private void GenerateSinglePDFQuestPdf(int index)
        {
            try
            {
                string outputDir = @"D:\Dotnet\etax-service\PDFs";
                string fileName = $"receipt_{DateTime.Now:yyyyMMddHHmmss}_{index}.pdf";
                string filePath = Path.Combine(outputDir, fileName);
                string logoPath = @"D:\Dotnet\etax-service\Images\logo_pea_1.png";

                Directory.CreateDirectory(outputDir);
                QuestPDF.Settings.License = LicenseType.Community;

                var document = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.DefaultTextStyle(x => x.FontSize(11));
                        
                        page.Size(PageSizes.A3);
                        page.Margin(4, Unit.Centimetre);

                        ConfigureHeader(page, logoPath);
                        ConfigureContent(page);
                        ConfigureFooter(page);
                    });
                });

                document.GeneratePdf(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating PDF {index}: {ex.Message}");
            }
        }

        private void ConfigureHeader(PageDescriptor page, string logoPath)
        {
            page.Header().Row(row =>
            {
                row.ConstantItem(80)
                   .PaddingTop(30)
                   .Element(container => container.Image(logoPath));

                row.RelativeItem()
                   .AlignCenter()
                   .Column(column =>
                   {
                       column.Item().AlignCenter().Text("ใบเสร็จรับเงิน")
                            .SemiBold()
                            .FontSize(20);
                       column.Item().AlignCenter().Text("e-Receipt")
                            .SemiBold()
                            .FontSize(16);
                   });

                row.ConstantItem(100);
            });
        }

        private void ConfigureContent(PageDescriptor page)
        {
            page.Content().Column(column =>
            {
                column.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem(6).Text(text =>
                    {
                        text.Span("การไฟฟ้าส่วนภูมิภาค (L1222)");
                    });
                    row.RelativeItem(4).Text(text =>
                    {
                        text.Span("เลขที่ (No.): R99810002706");
                    });
                });

                column.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem(6).Text(text =>
                    {
                        text.Span("300/25 ม.5 ต.พิมาน อ.เมือง จ.สงขลา 90170");
                    });
                    row.RelativeItem(4).Text(text =>
                    {
                        text.Span("วันที่ (Date): 08/03/2562");
                    });
                });

                column.Item().PaddingTop(20).Row(row =>
                {
                    row.RelativeItem(6).Text(text =>
                    {
                        text.Span("เลขประจำตัวผู้เสียภาษี (Tax ID No.): ");
                        text.Span("0994000165501");
                    });
                    row.RelativeItem(4).Text(text =>
                    {
                        text.Span("เลขที่ใบแจ้งหนี้ (Invoice No.): ");
                        text.Span("0000841001419692");
                    });
                });

                column.Item().PaddingTop(6).Row(row =>
                {
                    row.RelativeItem(6).Text(text =>
                    {
                        text.Span("ชื่อ (Name): ");
                        text.Span("นายทดสอบ ระบบทดสอบ");
                    });

                    row.RelativeItem(4).Text(text =>
                    {
                        text.Span("สาขาที่ (Branch No.): ");
                        text.Span("00344");
                    });
                });

                column.Item().PaddingTop(6).Row(row =>
                {
                    row.RelativeItem().Element(container =>
                    {
                        container.MaxWidth(350).Text(text =>
                        {
                            text.Span("ที่อยู่ (Address): ").SemiBold();
                            text.Span("133 ม.5 ถ.กาญจนวนิช-พญาไทย ต.พิมาน อ.เมือง จ.สงขลา 90170 ประเทศไทย");
                        });
                    });
                });

                ConfigureCustomerInfo(column);
                // ConfigureContractInfo(column);
            });
        }

        private void ConfigureCustomerInfo(ColumnDescriptor column)
        {
            column.Item().PaddingTop(20).Row(row =>
            {
                row.RelativeItem(4).Text(text =>
                {
                    text.Span("รหัสลูกค้า (Contract Account): ");
                    text.Span("020002705548");
                });

                row.RelativeItem(6).Text(text =>
                {
                    text.Span("รหัสการไฟฟ้า L12201 การไฟฟ้าส่วนภูมิภาคพังงา");
                    text.Span("00344");
                });
            });

            column.Item().PaddingTop(6).Text(text =>
            {
                text.Span("ชำระโดย (Account name) ");
                text.Span("นายลูกค้า คงทนดี");
            });

            column.Item().PaddingTop(6).Row(row =>
            {
                row.RelativeItem().Text(text =>
                {
                    text.Span("ที่อยู่ (Address): ").SemiBold();
                    text.Span("133 ม.5 ถ.กาญจนวนิช-พญาไทย ต.พิมาน อ.เมือง จ.สงขลา 90170 ประเทศไทย");
                });
            });

        }

        private void ConfigureContractInfo(ColumnDescriptor column)
        {
            column.Item().PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Text(text =>
                {
                    text.Span("ชำระโดย (Account name)").SemiBold();
                    text.Span("นายลูกค้า คงทนดี");
                });
            });
        }

        private void ConfigureFooter(PageDescriptor page)
        {
            page.Footer()
                .AlignCenter()
                .Text(text =>
                {
                    text.Span("หน้า ").FontSize(10);
                    text.CurrentPageNumber().FontSize(10);
                    text.Span(" / ").FontSize(10);
                    text.TotalPages().FontSize(10);
                });
        }

        // --------- Generate QuestPDF ---------

        // --------- Generate PDFsharpCore -----

        public void GeneratePDFsharpCore()
        {
            Task.Run(async () =>
            {
                try
                {
                    await _semaphore.WaitAsync();
                    var startTime = DateTime.Now; 
                    
                    try
                    {
                        var fontResolver = new PrivateFontResolver();
                        GlobalFontSettings.FontResolver = fontResolver;

                        int count = 1;
                        int batchSize = 100;
                        
                        for (int start = 0; start < count; start += batchSize)
                        {
                            var tasks = new List<Task>();
                            int end = Math.Min(start + batchSize, count);

                            for (int i = start; i < end; i++)
                            {
                                int index = i;
                                tasks.Add(Task.Run(() => 
                                {
                                    GenerateSinglePDFsharpCore(index);
                                    Console.WriteLine($"Generated PDF {index + 1} of {count}");
                                }));
                            }

                            Task.WaitAll(tasks.ToArray());
                            Console.WriteLine($"Completed batch: {start + 1} to {end}");
                        }

                        Console.WriteLine($"Completed all PDFs using PDFsharp. Total time: {DateTime.Now - startTime}");  
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }

        private void GenerateSinglePDFsharpCore(int index)
        {
            try
            {
                string outputDir = @"D:\Dotnet\etax-service\PDFs";
                string fileName = $"receipt_pdfsharp_{DateTime.Now:yyyyMMddHHmmss}_{index}.pdf";
                string filePath = Path.Combine(outputDir, fileName);
                string logoPath = @"D:\Dotnet\etax-service\Images\logo_pea_1.png";

                Directory.CreateDirectory(outputDir);
                
                using (var document = new PdfSharpCore.Pdf.PdfDocument())
                {
                    var page = document.AddPage();
                    
                    page.Size = PdfSharpCore.PageSize.A3;
                    
                    using (XGraphics gfx = XGraphics.FromPdfPage(page))
                    {
                        var font = new XFont("THSarabunNew", 11);
                        var fontBold = new XFont("THSarabunNew", 11, XFontStyle.Bold);
                        var titleFont = new XFont("THSarabunNew", 20, XFontStyle.Bold);
                        var subtitleFont = new XFont("THSarabunNew", 16, XFontStyle.Bold);

                        double margin = 113;
                        double yPosition = margin;
                        
                        if (File.Exists(logoPath))
                        {
                            using (XImage image = XImage.FromFile(logoPath))
                            {
                                gfx.DrawImage(image, margin, yPosition, 80, 80);
                            }
                        }

                        gfx.DrawString("ใบเสร็จรับเงิน", titleFont, XBrushes.Black, 
                            new XRect(0, yPosition + 30, page.Width.Point, 30), XStringFormats.Center);
                        gfx.DrawString("e-Receipt", subtitleFont, XBrushes.Black, 
                            new XRect(0, yPosition + 60, page.Width.Point, 30), XStringFormats.Center);

                        yPosition += 150;

                        gfx.DrawString("การไฟฟ้าส่วนภูมิภาค (L1222)", font, XBrushes.Black, margin, yPosition);
                        gfx.DrawString("เลขที่ (No.): R99810002706", font, XBrushes.Black, page.Width.Point - margin - 200, yPosition);

                        yPosition += 20;
                        gfx.DrawString("300/25 ม.5 ต.พิมาน อ.เมือง จ.สงขลา 90170", font, XBrushes.Black, margin, yPosition);
                        gfx.DrawString("วันที่ (Date): 08/03/2562", font, XBrushes.Black, page.Width.Point - margin - 200, yPosition);

                        yPosition += 40;
                        gfx.DrawString("เลขประจำตัวผู้เสียภาษี (Tax ID No.): 0994000165501", font, XBrushes.Black, margin, yPosition);
                        gfx.DrawString("เลขที่ใบแจ้งหนี้ (Invoice No.): 0000841001419692", font, XBrushes.Black, page.Width.Point - margin - 300, yPosition);

                        yPosition += 20;
                        gfx.DrawString("ชื่อ (Name): นายทดสอบ ระบบทดสอบ", font, XBrushes.Black, margin, yPosition);
                        gfx.DrawString("สาขาที่ (Branch No.): 00344", font, XBrushes.Black, page.Width.Point - margin - 200, yPosition);

                        yPosition += 20;
                        gfx.DrawString("ที่อยู่ (Address): ", fontBold, XBrushes.Black, margin, yPosition);
                        gfx.DrawString("133 ม.5 ถ.กาญจนวนิช-พญาไทย ต.พิมาน อ.เมือง จ.สงขลา 90170 ประเทศไทย", 
                            font, XBrushes.Black, margin + 80, yPosition);

                        yPosition += 40;
                        gfx.DrawString("รหัสลูกค้า (Contract Account): 020002705548", font, XBrushes.Black, margin, yPosition);
                        gfx.DrawString("รหัสการไฟฟ้า L12201 การไฟฟ้าส่วนภูมิภาคพังงา 00344", 
                            font, XBrushes.Black, page.Width.Point - margin - 400, yPosition);

                        yPosition += 20;
                        gfx.DrawString("ชำระโดย (Account name) นายลูกค้า คงทนดี", font, XBrushes.Black, margin, yPosition);

                        yPosition += 20;
                        gfx.DrawString("ที่อยู่ (Address): ", fontBold, XBrushes.Black, margin, yPosition);
                        gfx.DrawString("133 ม.5 ถ.กาญจนวนิช-พญาไทย ต.พิมาน อ.เมือง จ.สงขลา 90170 ประเทศไทย", 
                            font, XBrushes.Black, margin + 80, yPosition);

                        string pageNumber = $"หน้า {1} / {1}";
                        var pageNumberSize = gfx.MeasureString(pageNumber, font);
                        gfx.DrawString(pageNumber, font, XBrushes.Black, 
                            (page.Width.Point - pageNumberSize.Width) / 2, page.Height.Point - margin);
                    }

                    document.Save(filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating PDFsharp {index}: {ex.Message}");
            }
        }

        public class PrivateFontResolver : IFontResolver
        {
            private static string FontPath = @"D:\Dotnet\etax-service\Fonts\THSarabunNew.ttf";

            public string DefaultFontName => "THSarabunNew";

            public byte[] GetFont(string faceName)
            {
                if (faceName.Equals("THSarabunNew", StringComparison.OrdinalIgnoreCase))
                {
                    return File.ReadAllBytes(FontPath);
                }
                return null;
            }

            public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
            {
                if (familyName.Equals("THSarabunNew", StringComparison.OrdinalIgnoreCase))
                {
                    return new FontResolverInfo("THSarabunNew");
                }
                return null;
            }
        }

        // --------- Generate PDFsharpCore -----

        // --------- Generate PDFItext7 -------
        
        public void GeneratePDFItext7()
        {
            Task.Run(async () =>
            {
                try
                {
                    await _semaphore.WaitAsync();
                    var startTime = DateTime.Now; 
                    
                    try
                    {
                        int count = 1;
                        int batchSize = 100;
                        
                        for (int start = 0; start < count; start += batchSize)
                        {
                            var tasks = new List<Task>();
                            int end = Math.Min(start + batchSize, count);

                            for (int i = start; i < end; i++)
                            {
                                int index = i;
                                tasks.Add(Task.Run(() => 
                                {
                                    GenerateSinglePDFItext7(index);
                                    Console.WriteLine($"Generated PDF {index + 1} of {count}");
                                }));
                            }

                            Task.WaitAll(tasks.ToArray());
                            Console.WriteLine($"Completed batch: {start + 1} to {end}");
                        }

                        Console.WriteLine($"Completed all PDFs using PDFsharp. Total time: {DateTime.Now - startTime}");  
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }

        private void GenerateSinglePDFItext7(int index)
        {
            try
            {
                string outputDir = @"D:\Dotnet\etax-service\PDFs";
                string fileName = $"receipt_itext7_{DateTime.Now:yyyyMMddHHmmss}_{index}.pdf"; 
                string filePath = Path.Combine(outputDir, fileName);
                string logoPath = @"D:\Dotnet\etax-service\Images\logo_pea_1.png";
                string fontPath = @"D:\Dotnet\etax-service\Fonts\THSarabunNew.ttf";

                Directory.CreateDirectory(outputDir);

                var writerProperties = new iText.Kernel.Pdf.WriterProperties()
                    .SetCompressionLevel(9)
                    .UseSmartMode();

                using (var writer = new iText.Kernel.Pdf.PdfWriter(filePath, writerProperties))
                using (var pdf = new iText.Kernel.Pdf.PdfDocument(writer))
                using (var document = new iText.Layout.Document(pdf, iText.Kernel.Geom.PageSize.A3))
                {
                    var font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
                    document.SetMargins(113, 113, 113, 113);

                    // Header
                    if (File.Exists(logoPath))
                    {
                        var imageData = ImageDataFactory.Create(logoPath);
                        var image = new iText.Layout.Element.Image(imageData)
                            .SetWidth(80)
                            .SetHeight(80)
                            .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);
                        document.Add(image);
                    }

                    var title = new iText.Layout.Element.Paragraph("ใบเสร็จรับเงิน")
                        .SetFont(font)
                        .SetFontSize(20)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .SetMarginTop(30);
                    document.Add(title);

                    var subtitle = new iText.Layout.Element.Paragraph("e-Receipt")
                        .SetFont(font)
                        .SetFontSize(16)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                    document.Add(subtitle);

                    // Content
                    var companyInfo = new iText.Layout.Element.Table(2)
                        .UseAllAvailableWidth()
                        .SetMarginTop(20);

                    // Row 1
                    companyInfo.AddCell(new Cell().Add(new Paragraph("การไฟฟ้าส่วนภูมิภาค (L1222)")
                        .SetFont(font)
                        .SetFontSize(11))
                        .SetBorder(Border.NO_BORDER));
                    companyInfo.AddCell(new Cell().Add(new Paragraph("เลขที่ (No.): R99810002706")
                        .SetFont(font)
                        .SetFontSize(11))
                        .SetBorder(Border.NO_BORDER)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));

                    companyInfo.AddCell(new Cell().Add(new Paragraph("300/25 ม.5 ต.พิมาน อ.เมือง จ.สงขลา 90170")
                        .SetFont(font)
                        .SetFontSize(11))
                        .SetBorder(Border.NO_BORDER));
                    companyInfo.AddCell(new Cell().Add(new Paragraph("วันที่ (Date): 08/03/2562")
                        .SetFont(font)
                        .SetFontSize(11))
                        .SetBorder(Border.NO_BORDER)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));

                    document.Add(companyInfo);

                    // Tax Info
                    var taxInfo = new iText.Layout.Element.Table(2)
                        .UseAllAvailableWidth()
                        .SetMarginTop(20);

                    taxInfo.AddCell(new Cell().Add(new Paragraph("เลขประจำตัวผู้เสียภาษี (Tax ID No.): 0994000165501")
                        .SetFont(font)
                        .SetFontSize(11))
                        .SetBorder(Border.NO_BORDER));
                    taxInfo.AddCell(new Cell().Add(new Paragraph("เลขที่ใบแจ้งหนี้ (Invoice No.): 0000841001419692")
                        .SetFont(font)
                        .SetFontSize(11))
                        .SetBorder(Border.NO_BORDER)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));

                    document.Add(taxInfo);

                    // Customer Info
                    var customerInfo = new iText.Layout.Element.Table(2)
                        .UseAllAvailableWidth()
                        .SetMarginTop(6);

                    customerInfo.AddCell(new Cell().Add(new Paragraph("ชื่อ (Name): นายทดสอบ ระบบทดสอบ")
                        .SetFont(font)
                        .SetFontSize(11))
                        .SetBorder(Border.NO_BORDER));
                    customerInfo.AddCell(new Cell().Add(new Paragraph("สาขาที่ (Branch No.): 00344")
                        .SetFont(font)
                        .SetFontSize(11))
                        .SetBorder(Border.NO_BORDER)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));

                    document.Add(customerInfo);

                    // Address
                    var address = new Paragraph()
                        .Add(new Text("ที่อยู่ (Address): ")
                            .SetFont(font)
                            .SetTextRise(0.5f)
                            .SetFontSize(11.5f))
                        .Add(new Text("133 ม.5 ถ.กาญจนวนิช-พญาไทย ต.พิมาน อ.เมือง จ.สงขลา 90170 ประเทศไทย")
                            .SetFont(font))
                        .SetFontSize(11)
                        .SetMarginTop(6);
                    document.Add(address);

                    // Footer
                    document.ShowTextAligned(new iText.Layout.Element.Paragraph($"หน้า 1 / 1")
                        .SetFont(font)
                        .SetFontSize(10),
                        559, 20, 1, 
                        iText.Layout.Properties.TextAlignment.CENTER, 
                        iText.Layout.Properties.VerticalAlignment.BOTTOM, 
                        0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating iText7 PDF {index}: {ex.Message}");
            }
        }

        // --------- Generate PDFItext7 -------

        // --------- Generate PDFIronPdf ------

        public void GeneratePDFIronPdf()
        {
            Task.Run(async () =>
            {
                try
                {
                    await _semaphore.WaitAsync();
                    var startTime = DateTime.Now; 
                    
                    try
                    {
                        int count = 1;
                        int batchSize = 100;
                        
                        for (int start = 0; start < count; start += batchSize)
                        {
                            var tasks = new List<Task>();
                            int end = Math.Min(start + batchSize, count);

                            for (int i = start; i < end; i++)
                            {
                                int index = i;
                                tasks.Add(Task.Run(() => 
                                {
                                    GenerateSinglePDFIronPdf(index);
                                    Console.WriteLine($"Generated PDF {index + 1} of {count}");
                                }));
                            }

                            Task.WaitAll(tasks.ToArray());
                            Console.WriteLine($"Completed batch: {start + 1} to {end}");
                        }

                        Console.WriteLine($"Completed all PDFs using PDFsharp. Total time: {DateTime.Now - startTime}");  
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }

        private void GenerateSinglePDFIronPdf(int index)
        {
            try
            {
                string outputDir = @"D:\Dotnet\etax-service\PDFs";
                string fileName = $"receipt_ironpdf_{DateTime.Now:yyyyMMddHHmmss}_{index}.pdf"; 
                string filePath = Path.Combine(outputDir, fileName);
                string logoPath = @"D:\Dotnet\etax-service\Images\logo_pea_1.png";
                string fontPath = @"D:\Dotnet\etax-service\Fonts\THSarabunNew.ttf";

                Directory.CreateDirectory(outputDir);

                IronPdf.License.LicenseKey = "IRONSUITE.SUWAT.BCH.GMAIL.COM.27377-8BF285B3B1-PO2TS-IR7W4NJGMPTB-KNGAXZU7XVNW-FKYM64QJWT4E-6VT6G6OSP HND-2GYMRVOXWGCR-XMLS5GBUGGED-NCOVB4-TXSXP6KEQKGPEA-DEPLOYMENT.TRIAL-QT44SU.TRIAL.EXPIRES.24.APR.2025"; 

                var html = $@"
                <html>
                <head>
                    <style>
                        @font-face {{
                            font-family: 'THSarabunNew';
                            src: url('{fontPath}');
                        }}
                        body {{
                            font-family: 'THSarabunNew';
                            margin: 113px;
                            font-size: 11pt;
                        }}
                        .header {{
                            text-align: center;
                            margin-bottom: 30px;
                        }}
                        .logo {{
                            width: 80px;
                            height: 80px;
                        }}
                        .title {{
                            font-size: 20pt;
                            font-weight: bold;
                            margin: 30px 0 10px 0;
                        }}
                        .subtitle {{
                            font-size: 16pt;
                            font-weight: bold;
                        }}
                        .info-row {{
                            display: flex;
                            justify-content: space-between;
                            margin: 6px 0;
                        }}
                        .footer {{
                            text-align: center;
                            position: fixed;
                            bottom: 20px;
                            left: 0;
                            right: 0;
                            font-size: 10pt;
                        }}
                    </style>
                </head>
                <body>
                    <div class='header'>
                        <img src='{logoPath}' class='logo'/>
                        <div class='title'>ใบเสร็จรับเงิน</div>
                        <div class='subtitle'>e-Receipt</div>
                    </div>

                    <div class='info-row'>
                        <div>การไฟฟ้าส่วนภูมิภาค (L1222)</div>
                        <div>เลขที่ (No.): R99810002706</div>
                    </div>

                    <div class='info-row'>
                        <div>300/25 ม.5 ต.พิมาน อ.เมือง จ.สงขลา 90170</div>
                        <div>วันที่ (Date): 08/03/2562</div>
                    </div>

                    <div class='info-row' style='margin-top: 20px;'>
                        <div>เลขประจำตัวผู้เสียภาษี (Tax ID No.): 0994000165501</div>
                        <div>เลขที่ใบแจ้งหนี้ (Invoice No.): 0000841001419692</div>
                    </div>

                    <div class='info-row'>
                        <div>ชื่อ (Name): นายทดสอบ ระบบทดสอบ</div>
                        <div>สาขาที่ (Branch No.): 00344</div>
                    </div>

                    <div style='margin-top: 6px;'>
                        <strong>ที่อยู่ (Address):</strong> 133 ม.5 ถ.กาญจนวนิช-พญาไทย ต.พิมาน อ.เมือง จ.สงขลา 90170 ประเทศไทย
                    </div>

                    <div class='info-row' style='margin-top: 20px;'>
                        <div>รหัสลูกค้า (Contract Account): 020002705548</div>
                        <div>รหัสการไฟฟ้า L12201 การไฟฟ้าส่วนภูมิภาคพังงา 00344</div>
                    </div>

                    <div style='margin-top: 6px;'>
                        ชำระโดย (Account name) นายลูกค้า คงทนดี
                    </div>

                    <div style='margin-top: 6px;'>
                        <strong>ที่อยู่ (Address):</strong> 133 ม.5 ถ.กาญจนวนิช-พญาไทย ต.พิมาน อ.เมือง จ.สงขลา 90170 ประเทศไทย
                    </div>

                    <div class='footer'>
                        หน้า 1 / 1
                    </div>
                </body>
                </html>";

                // สร้าง PDF จาก HTML
                var renderer = new IronPdf.ChromePdfRenderer();
                renderer.RenderingOptions.PaperSize = IronPdf.Rendering.PdfPaperSize.A3;
                renderer.RenderingOptions.MarginTop = 0; 
                renderer.RenderingOptions.MarginBottom = 0;
                renderer.RenderingOptions.MarginLeft = 0;
                renderer.RenderingOptions.MarginRight = 0;
                
                var pdf = renderer.RenderHtmlAsPdf(html);
                pdf.SaveAs(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating IronPDF {index}: {ex.Message}");
            }
        }

        // --------- Generate PDFIronPdf ------

        // --------- Generate PDFSyncfusionPdf ------

        public void GeneratePDFSyncfusionPdf()
        {
            Task.Run(async () =>
            {
                try
                {
                    await _semaphore.WaitAsync();
                    var startTime = DateTime.Now; 
                    
                    try
                    {
                        int count = 1;
                        int batchSize = 100;
                        
                        for (int start = 0; start < count; start += batchSize)
                        {
                            var tasks = new List<Task>();
                            int end = Math.Min(start + batchSize, count);

                            for (int i = start; i < end; i++)
                            {
                                int index = i;
                                tasks.Add(Task.Run(() => 
                                {
                                    GenerateSinglePDFSyncfusionPdf(index);
                                    Console.WriteLine($"Generated PDF {index + 1} of {count}");
                                }));
                            }

                            Task.WaitAll(tasks.ToArray());
                            Console.WriteLine($"Completed batch: {start + 1} to {end}");
                        }

                        Console.WriteLine($"Completed all PDFs using PDFsharp. Total time: {DateTime.Now - startTime}");  
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }

        private void GenerateSinglePDFSyncfusionPdf(int index)
        {
            try
            {
                string outputDir = @"D:\Dotnet\etax-service\PDFs";
                string fileName = $"receipt_syncfusion_{DateTime.Now:yyyyMMddHHmmss}_{index}.pdf"; 
                string filePath = Path.Combine(outputDir, fileName);
                string logoPath = @"D:\Dotnet\etax-service\Images\logo_pea_1.png";
                string fontPath = @"D:\Dotnet\etax-service\Fonts\THSarabunNew.ttf";

                Directory.CreateDirectory(outputDir);

                // ระบุ namespace ให้ชัดเจน
                using (Syncfusion.Pdf.PdfDocument document = new Syncfusion.Pdf.PdfDocument())
                {
                    document.PageSettings.Size = Syncfusion.Pdf.PdfPageSize.A3;
                    
                    Syncfusion.Pdf.PdfPage page = document.Pages.Add();
                    
                    Syncfusion.Pdf.Graphics.PdfGraphics graphics = page.Graphics;

                    FileStream fontStream = new FileStream(fontPath, FileMode.Open, FileAccess.Read);
                    Syncfusion.Pdf.Graphics.PdfTrueTypeFont font = new Syncfusion.Pdf.Graphics.PdfTrueTypeFont(fontStream, 11);
                    Syncfusion.Pdf.Graphics.PdfTrueTypeFont fontBold = new Syncfusion.Pdf.Graphics.PdfTrueTypeFont(fontStream, 11, Syncfusion.Pdf.Graphics.PdfFontStyle.Bold);
                    Syncfusion.Pdf.Graphics.PdfTrueTypeFont titleFont = new Syncfusion.Pdf.Graphics.PdfTrueTypeFont(fontStream, 20, Syncfusion.Pdf.Graphics.PdfFontStyle.Bold);
                    Syncfusion.Pdf.Graphics.PdfTrueTypeFont subtitleFont = new Syncfusion.Pdf.Graphics.PdfTrueTypeFont(fontStream, 16, Syncfusion.Pdf.Graphics.PdfFontStyle.Bold);

                    float margin = 113;
                    float yPosition = margin;

                    // วาดโลโก้
                    if (File.Exists(logoPath))
                    {
                        using (FileStream fs = new FileStream(logoPath, FileMode.Open, FileAccess.Read))
                        {
                            Syncfusion.Pdf.Graphics.PdfBitmap image = new Syncfusion.Pdf.Graphics.PdfBitmap(fs);
                            graphics.DrawImage(image, margin, yPosition, 80, 80);
                        }
                    }

                    // หัวเอกสาร
                    graphics.DrawString("ใบเสร็จรับเงิน", titleFont, PdfBrushes.Black, 
                        new Syncfusion.Drawing.RectangleF(0, yPosition + 30, page.GetClientSize().Width, 30),
                        new PdfStringFormat { Alignment = PdfTextAlignment.Center });

                    graphics.DrawString("e-Receipt", subtitleFont, PdfBrushes.Black,
                        new Syncfusion.Drawing.RectangleF(0, yPosition + 60, page.GetClientSize().Width, 30),
                        new PdfStringFormat { Alignment = PdfTextAlignment.Center });

                    yPosition += 150;

                    // ข้อมูลบริษัท
                    graphics.DrawString("การไฟฟ้าส่วนภูมิภาค (L1222)", font, PdfBrushes.Black, margin, yPosition);
                    graphics.DrawString("เลขที่ (No.): R99810002706", font, PdfBrushes.Black, 
                        page.GetClientSize().Width - margin - 200, yPosition);

                    yPosition += 20;
                    graphics.DrawString("300/25 ม.5 ต.พิมาน อ.เมือง จ.สงขลา 90170", font, PdfBrushes.Black, margin, yPosition);
                    graphics.DrawString("วันที่ (Date): 08/03/2562", font, PdfBrushes.Black, 
                        page.GetClientSize().Width - margin - 200, yPosition);

                    yPosition += 40;
                    graphics.DrawString("เลขประจำตัวผู้เสียภาษี (Tax ID No.): 0994000165501", font, PdfBrushes.Black, margin, yPosition);
                    graphics.DrawString("เลขที่ใบแจ้งหนี้ (Invoice No.): 0000841001419692", font, PdfBrushes.Black, 
                        page.GetClientSize().Width - margin - 300, yPosition);

                    yPosition += 20;
                    graphics.DrawString("ชื่อ (Name): นายทดสอบ ระบบทดสอบ", font, PdfBrushes.Black, margin, yPosition);
                    graphics.DrawString("สาขาที่ (Branch No.): 00344", font, PdfBrushes.Black, 
                        page.GetClientSize().Width - margin - 200, yPosition);

                    yPosition += 20;
                    graphics.DrawString("ที่อยู่ (Address): ", fontBold, PdfBrushes.Black, margin, yPosition);
                    graphics.DrawString("133 ม.5 ถ.กาญจนวนิช-พญาไทย ต.พิมาน อ.เมือง จ.สงขลา 90170 ประเทศไทย", 
                        font, PdfBrushes.Black, margin + 80, yPosition);

                    // Footer
                    string pageNumber = $"หน้า 1 / 1";
                    Syncfusion.Drawing.SizeF pageNumberSize = font.MeasureString(pageNumber);
                    graphics.DrawString(pageNumber, font, PdfBrushes.Black,
                        (page.GetClientSize().Width - pageNumberSize.Width) / 2,
                        page.GetClientSize().Height - margin);

                    // บันทึกไฟล์ PDF
                    document.Save(filePath);
                    document.Close(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating Syncfusion PDF {index}: {ex.Message}");
            }
        }

        // --------- Generate PDFSyncfusionPdf ------
    }
}
