using EPiServer.ServiceLocation;
using Microsoft.Extensions.DependencyInjection;
using SeoBoost.Business.Url;

namespace SeoBoost.Business.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSeoBoost(this IServiceCollection services)
        {
            return AddHybridServices(services);
        }

        public static IServiceCollection AddHybridServices(this IServiceCollection services)
        {
            services.AddHttpContextOrThreadScoped<IUrlService, UrlService>();

            return services;
        }
    }
}