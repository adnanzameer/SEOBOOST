using EPiServer.Core;

namespace SeoBoost.Business.Url
{
    public interface IUrlService
    {
        string GetExternalFriendlyUrl(ContentReference contentReference, string culture);
        string GetExternalFriendlyUrl(ContentReference contentReference);
        string GetHost(ContentReference contentReference);
    }
}
