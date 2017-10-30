using System.Collections.Generic;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using SeoBoost.Business.Extension;

namespace SeoBoost.Models.ViewModels
{
    public class BreadcrumbsViewModel
    {
        public readonly List<BreadcrumbItemListElementViewModel> BreadcrumbItemList;

        private int _index = 1;

        public BreadcrumbsViewModel(PageData currentPage)
        {
            BreadcrumbItemList = GetBreadcrumbItemList(currentPage);
        }

        private List<BreadcrumbItemListElementViewModel> GetBreadcrumbItemList(PageData currentPage)
        {
            var breadcrumbItemList = new List<BreadcrumbItemListElementViewModel>();

            var startPageReference = ContentReference.StartPage;

            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();

            var startPage = contentLoader.Get<PageData>(startPageReference);
            var startPageBreadcrumbElement = GetPageBreadcrumbElement(startPage, false);
            breadcrumbItemList.Add(startPageBreadcrumbElement);

            if (currentPage != startPage)
            {
                var root = ContentReference.RootPage;
                var parents = new List<PageData>();

                GetParentBreadcrumbs(currentPage, ref parents);

                parents.Reverse();

                foreach (var parent in parents)
                {
                    if (parent.PageLink.ID == root.ID) continue;
                    if (parent.PageLink.ID == startPageReference.ID) continue;

                    breadcrumbItemList.Add(GetPageBreadcrumbElement(parent, false));
                }
                breadcrumbItemList.Add(GetPageBreadcrumbElement(currentPage, true));

            }

            return breadcrumbItemList;
        }

        private ICollection<PageData> GetParentBreadcrumbs(PageData page, ref List<PageData> parents)
        {
            var parent = page.GetParent();

            if (parent != null)
            {
                if (parent.CheckPublishedStatus(PagePublishedStatus.Published))
                {
                    parents.Add(parent);
                }

                return GetParentBreadcrumbs(parent, ref parents);
            }

            return parents;
        }
        private BreadcrumbItemListElementViewModel GetPageBreadcrumbElement(PageData page, bool selected)
        {
            string currentPageName = page.Name;

            var breadcrumbCurrentPageElement = new BreadcrumbItemListElementViewModel(
                currentPageName,
                page.ContentLink.GetExternalUrl(),
                IncrementIndex(),
                selected, page.VisibleInMenu);

            return breadcrumbCurrentPageElement;
        }

        private int IncrementIndex()
        {
            return _index++;
        }
    }
}
