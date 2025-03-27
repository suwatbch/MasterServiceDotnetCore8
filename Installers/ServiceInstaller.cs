using System.Reflection;
using EtaxService.Configuration;

namespace EtaxService.Installers
{
    public static class ServiceInstaller
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            // หา Interface ที่ลงท้ายด้วย Service
            var serviceTypes = assembly.GetTypes()
                .Where(t => t.IsInterface && t.Name.EndsWith("Service"))
                .ToList();

            foreach (var serviceType in serviceTypes)
            {
                // หา Implementation class และตรวจสอบว่ามีเพียงตัวเดียว
                var implementations = assembly.GetTypes()
                    .Where(t => t.IsClass 
                        && !t.IsAbstract 
                        && serviceType.IsAssignableFrom(t))
                    .ToList();

                if (implementations.Count == 1)
                {
                    services.AddScoped(serviceType, implementations.First());
                }
                else if (implementations.Count > 1)
                {
                    throw new InvalidOperationException(
                        $"Found multiple implementations of {serviceType.Name}: {string.Join(", ", implementations.Select(t => t.Name))}");
                }
                else
                {
                    throw new InvalidOperationException(
                        $"No implementation found for {serviceType.Name}");
                }
            }

            return services;
        }

        public static IServiceCollection AddApplicationRepositories(this IServiceCollection services)
        {
            // ลงทะเบียน Generic Repository
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

            var assembly = Assembly.GetExecutingAssembly();
            
            // หา Interface ที่ลงท้ายด้วย Repository
            var repositoryTypes = assembly.GetTypes()
                .Where(t => t.IsInterface 
                    && t.Name.EndsWith("Repository")
                    && t != typeof(IRepository<>)) // ยกเว้น Generic Repository
                .ToList();

            foreach (var repositoryType in repositoryTypes)
            {
                // หา Implementation class และตรวจสอบว่ามีเพียงตัวเดียว
                var implementations = assembly.GetTypes()
                    .Where(t => t.IsClass 
                        && !t.IsAbstract 
                        && repositoryType.IsAssignableFrom(t)
                        && t != typeof(GenericRepository<>)) // ยกเว้น Generic Repository
                    .ToList();

                if (implementations.Count == 1)
                {
                    services.AddScoped(repositoryType, implementations.First());
                }
                else if (implementations.Count > 1)
                {
                    throw new InvalidOperationException(
                        $"Found multiple implementations of {repositoryType.Name}: {string.Join(", ", implementations.Select(t => t.Name))}");
                }
                else
                {
                    throw new InvalidOperationException(
                        $"No implementation found for {repositoryType.Name}");
                }
            }

            return services;
        }
    }
} 