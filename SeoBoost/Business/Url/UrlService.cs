using EPiServer.Core;

namespace SeoBoost.Business.Url
{
    public class UrlService : IUrlService
    {
        public string GetExternalFriendlyUrl(ContentReference contentReference, string culture)
        {
            var builder = new UrlBuilder(contentReference, culture);
            builder.AddMissingPathPart();
            return builder.GetExternalUrl();
        }

        public string GetExternalFriendlyUrl(ContentReference contentReference)
        {
            var builder = new UrlBuilder(contentReference);
            builder.AddMissingPathPart();
            return builder.GetExternalUrl();
        }

        public string GetHost(ContentReference contentReference)
        {
            var builder = new UrlBuilder(contentReference);
            return builder.GetExternalUrlHost();
        }
    }
}
