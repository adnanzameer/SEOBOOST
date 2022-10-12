using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using SeoBoost.Business.Url;
using SeoBoost.Models;
using SeoBoost.Models.ViewModels;

namespace SeoBoost.Helper
{
    public static class SeoHelper
    {
        public static HtmlString GetAlternateLinks(this IHtmlHelper html)
        {
            var requestContext = html.ViewContext.HttpContext;
            var contentReference = requestContext.GetContentLink();

            return GetAlternateLinks(contentReference);
        }

        public static HtmlString GetAlternateLinks(this ContentReference contentReference)
        {
            if (!ProcessRequest)
                return new HtmlString("");

            var alternates = new List<AlternativePageLink>();
            var languages = ServiceLocator.Current.GetInstance<ILanguageBranchRepository>().ListEnabled();

            GetAlternativePageLink(contentReference, languages, alternates);

            var masterLanguage = languages.FirstOrDefault(l => l.ID == 1) ?? languages.FirstOrDefault();

            var xDefault =
                alternates.FirstOrDefault(a => string.Equals(a.Culture.ToLower(), masterLanguage?.LanguageID.ToLower()));

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
                    if (fallbackLanguages.Any() && fallbackLanguages.Contains(masterLanguage?.LanguageID))
                    {
                        var url = xDefault.Url.Replace("/" + masterLanguage?.LanguageID.ToLower() + "/",
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

        public static HtmlString GetAlternateLinks(this PageData pageData)
        {
            return GetAlternateLinks(pageData.ContentLink);
        }

        public static HtmlString GetCanonicalLink(this IHtmlHelper html)
        {
            var requestContext = html.ViewContext.HttpContext;
            var contentReference = requestContext.GetContentLink();
            return GetCanonicalLink(contentReference);
        }

        public static HtmlString GetCanonicalLink(this ContentReference contentReference)
        {
            if (!ProcessRequest)
                return new HtmlString("");

            var sb = new StringBuilder();
            var urlService = ServiceLocator.Current.GetInstance<IUrlService>();
            sb.AppendLine("<link rel=\"canonical\" href=\"" + urlService.GetExternalFriendlyUrl(contentReference) +
                          "\" />");
            return new HtmlString(sb.ToString());
        }

        public static HtmlString GetCanonicalLink(this PageData pageData)
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
            if (IsBlockContext)
                return new List<BreadcrumbItemListElementViewModel>();

            if (pageData == null)
                return new List<BreadcrumbItemListElementViewModel>();

            var reference = startPageReference;
            if (reference == null || ContentReference.IsNullOrEmpty(reference))
                reference = ContentReference.StartPage;

            var breadcrumbModel = new BreadcrumbsViewModel(pageData, reference);
            return breadcrumbModel.BreadcrumbItemList;
        }

        public static List<BreadcrumbItemListElementViewModel> GetBreadcrumbItemList(this IHtmlHelper html, ContentReference startPageReference = null)
        {
            var requestContext = html.ViewContext.HttpContext;
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
            {
                foreach (var p in pagesData)
                {
                    if (string.Equals(p.Language.Name.ToLower(), language.LanguageID.ToLower(),
                        StringComparison.Ordinal))
                    {
                        var url = urlService.GetExternalFriendlyUrl(page.ContentLink, p.Language.Name);
                        var alternate = new AlternativePageLink(url, language.LanguageID);

                        alternates.Add(alternate);
                        break;
                    }
                }
            }
        }

        private static HtmlString CreateHtmlString(AlternativeLinkViewModel model)
        {
            var sb = new StringBuilder();

            foreach (var alternate in model.Alternates)
            {
                sb.AppendLine("<link rel=\"alternate\" href=\"" + alternate.Url + "\" hreflang=\"" + alternate.Culture.ToLower() + "\" />");
            }

            if (!string.IsNullOrEmpty(model.XDefaultUrl))
            {
                sb.AppendLine(" <link rel=\"alternate\" href=\"" + model.XDefaultUrl + "\" hreflang=\"x-default\" />");
            }

            return new HtmlString(sb.ToString());
        }

        private static bool ProcessRequest
        {
            get
            {
                var process = !IsInEditMode();

                if (process)
                {
                    process = !IsBlockContext;
                }

                return process;

            }
        }

        private static bool IsInEditMode()
        {
            var contextModeResolver = ServiceLocator.Current.GetInstance<IContextModeResolver>();
            var mode = contextModeResolver.CurrentMode;
            return mode is ContextMode.Edit or ContextMode.Preview;
        }

        private static bool IsBlockContext
        {
            get
            {
                var contentRouteHelper = ServiceLocator.Current.GetInstance<IContentRouteHelper>();
                return contentRouteHelper.Content is BlockData;
            }
        }

        //internal static async Task AddRoute()
        //{
        //   await RemoveRoute();

        //    var route = RouteTable.Routes.MapRoute(
        //        "RobotsTxtRoute",
        //        "robots.txt",
        //        (object)new { controller = "SBRobotsTxt", action = "Index" });

        //    RouteTable.Routes.Remove(route);
        //    RouteTable.Routes.Insert(0, route);
        //}

        //internal static async Task RemoveRoute()
        //{
        //    await Task.Run(() =>
        //    {
        //        var index = RouteIndex();
        //        while (index != -1)
        //        {
        //            RouteTable.Routes.RemoveAt(index);
        //            index = RouteIndex();
        //        }
        //    });
        //}

        //private static int RouteIndex()
        //{
        //    for (var i = 0; i < RouteTable.Routes.Count; i++)
        //    {
        //        var routeBaseItem = RouteTable.Routes[i];
        //        if (routeBaseItem is Route routeData && routeData.Url.Contains("robots.txt"))
        //        {
        //            return i;
        //        }

        //    }

        //    return -1;
        //}
    }
}