using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using EtaxService.Database;
using EtaxService.Services;
using EtaxService.Repositories;
using EtaxService.Configuration;
using EtaxService.Installers;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;

namespace EtaxService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // ลงทะเบียน PDFsharpCore
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // ลงทะเบียน Syncfusion
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NNaF5cXmBCekx0QXxbf1x1ZFRGal9WTnRXUj0eQnxTdEBjWn1bcnZQQ2FZVEBzXklfag==");
            
            var builder = WebApplication.CreateBuilder(args);

            // 1. การตั้งค่า Configuration
            ConfigureAppSettings(builder);

            // 2. การตั้งค่า Services หลัก
            ConfigureMainServices(builder);

            // 3. การตั้งค่า Swagger
            builder.Services.AddSwaggerConfiguration();

            // 4. การตั้งค่า Database
            builder.Services.ConfigureDatabase(builder.Configuration);

            // 5. การตั้งค่า JWT และ Authentication
            var jwtSettings = builder.Services.ConfigureJWT(builder.Configuration);

            // 6. การลงทะเบียน Dependencies
            ConfigureDependencyInjection(builder);

            // 7. การตั้งค่า Middleware
            var app = builder.Build();
            ConfigureMiddleware(app);

            app.Run();
        }

        private static void ConfigureAppSettings(WebApplicationBuilder builder)
        {
            builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
            // builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
        }

        private static void ConfigureMainServices(WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    policy.WithOrigins("http://localhost:3000") // Allow requests from your frontend
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<HeadersAttribute>();
            });
        }

        private static void ConfigureDependencyInjection(WebApplicationBuilder builder)
        {
            builder.Services
                .AddApplicationRepositories()
                .AddApplicationServices();
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("AllowSpecificOrigin");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}
