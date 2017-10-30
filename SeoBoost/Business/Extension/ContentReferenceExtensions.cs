using EPiServer.Core;
using EPiServer.ServiceLocation;
using SeoBoost.Business.Url;

namespace SeoBoost.Business.Extension
{
    public static class ContentReferenceExtensions
    {
        public static string GetExternalUrl(this ContentReference contentReference)
        {
            IUrlService urlService = ServiceLocator.Current.GetInstance<IUrlService>();
            return urlService.GetExternalUrl(contentReference);
        }
    }
}
