using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace SeoBoost.Helper
{
    public static class GenericFunctions
    {
        public static string GetFriendlyUrl(ContentReference pageLink, string culture)
        {
            return ServiceLocator.Current.GetInstance<UrlResolver>()
                .GetUrl(pageLink, culture);
        }
    }
}
