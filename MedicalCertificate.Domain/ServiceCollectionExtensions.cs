using MedicalCertificate.Domain.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalCertificate.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions(configuration);
        
        return services;
    }
    
    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtConfigurationOptions>(
            configuration.GetSection(JwtConfigurationOptions.SectionName));
        
        return services;
    }
}