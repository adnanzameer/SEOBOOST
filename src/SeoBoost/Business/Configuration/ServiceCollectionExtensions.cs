using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SeoBoost.Business.Events;
using SeoBoost.Business.Initialization;
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
            services.AddTransient<IViewTemplateModelRegistrator, TemplateCoordinator>();
            services.AddSingleton<SeoBoostInitializer>();

            return services;
        }
    }
}