using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Microsoft.Extensions.DependencyInjection;
using SeoBoost.Business.Url;

namespace SeoBoost.Business.DependencyResolver
{
    [InitializableModule]
    internal class DependencyResolver : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            var configurationProvider = context.Services;
            AddHybridServices(configurationProvider);
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        private static void AddHybridServices(IServiceCollection configurationProvider)
        {
            configurationProvider.AddHttpContextOrThreadScoped<IUrlService, UrlService>();
        }
    }
}