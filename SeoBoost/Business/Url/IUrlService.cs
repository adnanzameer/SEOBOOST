using EPiServer.Core;

namespace SeoBoost.Business.Url
{
    public interface IUrlService
    {
        string GetExternalUrl(ContentReference contentReference);
        string GetExternalFriendlyUrl(ContentReference contentReference, string culture);
        string GetExternalFriendlyUrl(ContentReference contentReference);
    }
}