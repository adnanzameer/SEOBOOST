using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace SeoBoost.Business.Extension
{
    public static class PageDataExtensions
    {
        public static PageData GetParent(this PageData currentPage)
        {
            if (currentPage.ParentLink == PageReference.EmptyReference)
                return null;

            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            return contentLoader.Get<PageData>(currentPage.ParentLink);
        }

        public static TResult GetParent<TResult>(this PageData currentPage)
            where TResult : class
        {
            if (PageReference.IsNullOrEmpty(currentPage.ParentLink))
                return null;

            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var parent = contentLoader.Get<PageData>(currentPage.ParentLink);
            if (parent == null)
                return null;

            if (parent is TResult)
                return parent as TResult;

            return GetParent<TResult>(parent);
        }
    }
}