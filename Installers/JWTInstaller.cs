using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EtaxService.Configuration;

namespace EtaxService.Installers
{
    public static class JWTInstaller
    {
        public static JwtSettings ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = new JwtSettings();
            configuration.GetSection("JwtSettings").Bind(jwtSettings);
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            
            ConfigureAuthentication(services, jwtSettings);
            
            return jwtSettings;
        }

        private static void ConfigureAuthentication(IServiceCollection services, JwtSettings jwtSettings)
        {
            var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });

            services.AddAuthorization();
        }
    }
} 