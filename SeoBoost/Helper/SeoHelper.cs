using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Editor;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using SeoBoost.Business.Url;
using SeoBoost.Models;
using SeoBoost.Models.ViewModels;

namespace SeoBoost.Helper
{
    public static class SeoHelper
    {
        public static MvcHtmlString GetAlternateLinks(this HtmlHelper html)
        {
            var requestContext = html.ViewContext.RequestContext;
            var contentReference = requestContext.GetContentLink();

            return GetAlternateLinks(contentReference);
        }

        public static MvcHtmlString GetAlternateLinks(this ContentReference contentReference)
        {
            if (!ProcessRequest)
                return MvcHtmlString.Create("");

            var alternates = new List<AlternativePageLink>();
            var languages = ServiceLocator.Current.GetInstance<ILanguageBranchRepository>().ListEnabled();

            GetAlternativePageLink(contentReference, languages, alternates);

            var masterLanguage = languages.FirstOrDefault(l => l.ID == 1) ?? languages.FirstOrDefault();

            var xDefault =
                alternates.FirstOrDefault(a => string.Equals(a.Culture.ToLower(), masterLanguage.LanguageID.ToLower()));

            var siteLanguageList = languages.Select(language => language.LanguageID).ToList();

            #region For fallback language page links 

            foreach (var link in alternates)
                if (siteLanguageList.Contains(link.Culture))
                    siteLanguageList.Remove(link.Culture);

            foreach (var language in siteLanguageList)
                if (xDefault != null && language != null)
                {
                    var fallbackLanguages =
                        ContentLanguageSettingsHandler.Instance.GetFallbackLanguages(contentReference, language);
                    if (fallbackLanguages.Any() && fallbackLanguages.Contains(masterLanguage.LanguageID))
                    {
                        var url = xDefault.Url.Replace("/" + masterLanguage.LanguageID.ToLower() + "/",
                            "/" + language + "/");
                        var alternate = new AlternativePageLink(url, language);
                        alternates.Add(alternate);
                    }
                }

            #endregion

            var model = new AlternativeLinkViewModel(alternates);
            if (!string.IsNullOrEmpty(xDefault?.Url))
                model.XDefaultUrl = xDefault.Url;
            var htmlString = CreateHtmlString(model);

            return htmlString;
        }

        public static MvcHtmlString GetAlternateLinks(this PageData pageData)
        {
            return GetAlternateLinks(pageData.ContentLink);
        }

        public static MvcHtmlString GetCanonicalLink(this HtmlHelper html)
        {
            var requestContext = html.ViewContext.RequestContext;
            var contentReference = requestContext.GetContentLink();
            return GetCanonicalLink(contentReference);
        }

        public static MvcHtmlString GetCanonicalLink(this ContentReference contentReference)
        {
            if (!ProcessRequest)
                return MvcHtmlString.Create("");

            var sb = new StringBuilder();
            var urlService = ServiceLocator.Current.GetInstance<IUrlService>();
            sb.AppendLine("<link rel=\"canonical\" href=\"" + urlService.GetExternalFriendlyUrl(contentReference) +
                          "\" />");
            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString GetCanonicalLink(this PageData pageData)
        {
            return GetCanonicalLink(pageData.ContentLink);
        }

        public static List<BreadcrumbItemListElementViewModel> GetBreadcrumbItemList(
            this ContentReference contentReference, ContentReference startPageReference = null)
        {
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var pageData = contentLoader.Get<IContent>(contentReference) as PageData;

            return GetBreadcrumbItemList(pageData, startPageReference);
        }

        public static List<BreadcrumbItemListElementViewModel> GetBreadcrumbItemList(this PageData pageData, ContentReference startPageReference = null)
        {
            if(IsBlockContext)
                return new List<BreadcrumbItemListElementViewModel>();

            if (pageData == null)
                return new List<BreadcrumbItemListElementViewModel>();

            var reference = startPageReference;
            if (reference == null || ContentReference.IsNullOrEmpty(reference))
                reference = ContentReference.StartPage;

            var breadcrumbModel = new BreadcrumbsViewModel(pageData, reference);
            return breadcrumbModel.BreadcrumbItemList;
        }

        public static List<BreadcrumbItemListElementViewModel> GetBreadcrumbItemList(this HtmlHelper html, ContentReference startPageReference = null)
        {
            var requestContext = html.ViewContext.RequestContext;
            var contentReference = requestContext.GetContentLink();

            return GetBreadcrumbItemList(contentReference, startPageReference);
        }

        private static void GetAlternativePageLink(ContentReference contentReference, IList<LanguageBranch> languages,
            List<AlternativePageLink> alternates)
        {
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var page = contentRepository.Get<IContent>(contentReference) as PageData;

            if (page == null)
                return;

            var pageLanguages = contentRepository.GetLanguageBranches<PageData>(page.ContentLink);
            var urlService = ServiceLocator.Current.GetInstance<IUrlService>();

            var pagesData = pageLanguages as IList<PageData> ?? pageLanguages.ToList();


            foreach (var language in languages)
                foreach (var p in pagesData)
                {
                    if (string.Equals(p.LanguageBranch.ToLower(), language.LanguageID.ToLower(),
                        StringComparison.Ordinal))
                    {
                        var url = urlService.GetExternalFriendlyUrl(page.ContentLink, p.LanguageBranch);
                        var alternate = new AlternativePageLink(url, language.LanguageID);

                        alternates.Add(alternate);
                        break;
                    }
                }
        }

        private static MvcHtmlString CreateHtmlString(AlternativeLinkViewModel model)
        {
            var sb = new StringBuilder();

            foreach (var alternate in model.Alternates)
                sb.AppendLine("<link rel=\"alternate\" href=\"" + alternate.Url + "\" hreflang=\"" + alternate.Culture.ToLower() +
                              "\" />");

            if (!string.IsNullOrEmpty(model.XDefaultUrl))
                sb.AppendLine(" <link rel=\"alternate\" href=\"" + model.XDefaultUrl + "\" hreflang=\"x-default\" />");

            return MvcHtmlString.Create(sb.ToString());
        }

        private static bool ProcessRequest
        {
            get
            {
                bool process = !PageEditing.PageIsInEditMode;

                if (process)
                {
                    process = !IsBlockContext;
                }

                return process;

            }
        }

        private static bool IsBlockContext
        {
            get
            {
                var contentRouteHelper = ServiceLocator.Current.GetInstance<IContentRouteHelper>();
                var content = contentRouteHelper.Content as BlockData;
                if (content != null)
                    return true;

                return false;
            }
        }
    }
}