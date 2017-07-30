using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using SeoBoost.Models;
using SeoBoost.ViewModels;

namespace SeoBoost.Helper
{
    public static class SeoHelper
    {
        public static MvcHtmlString HrefLang()
        {
            var pageRouteHelper = ServiceLocator.Current.GetInstance<IPageRouteHelper>();

            var currentPageLink = pageRouteHelper.PageLink;

            var startPageReference = ContentReference.StartPage;

            var isStartPage = startPageReference.ID == currentPageLink.ID;

            var alternates = new List<AlternativePageLink>();

            var domain = SiteDefinition.Current.SiteUrl.ToString();

            if (domain.EndsWith("/"))
                domain = domain.Substring(0, domain.Length - 1);

            var languages = DataFactory.Instance.GetLanguageBranches(currentPageLink);

            foreach (var language in languages)
            {
                foreach (var p in DataFactory.Instance.GetLanguageBranches(currentPageLink))
                {
                    if (p.LanguageBranch.ToLower() == language.LanguageBranch.ToLower())
                    {
                        var url = GenericFunctions.GetFriendlyUrl(currentPageLink, p.LanguageBranch);
                        var host = domain;
                        var pageLanguage = p.Language;

                        foreach (var a in SiteDefinition.Current.Hosts)
                        {
                            if (a.Language != null && a.Language.Equals(pageLanguage))
                            {
                                var siteSchema = a.UseSecureConnection.HasValue && a.UseSecureConnection.Value ? "https" : "http";
                                host = siteSchema + "://" + a.Name;
                                break;
                            }
                        }

                        if (!string.IsNullOrEmpty(url) && url.ToLower().Contains("//"))
                        {
                            host = "";
                        }

                        var alternate = new AlternativePageLink(url, string.Format("{0}{1}", host, url), language.LanguageID);

                        alternates.Add(alternate);
                        break;
                    }
                }
            }

            var model = new HrefLangViewModel(isStartPage, domain, alternates);

            var sb = new StringBuilder();
            foreach (var alternate in model.Alternates)
            {
                sb.AppendLine("<link rel=\"alternate\" href=\"" + alternate.Url + "\" hreflang=\"" + alternate.Culture + "\" />");
            }

            if (model.IsStartPage)
            {
                sb.AppendLine(" <link rel=\"alternate\" href=\"" + model.Domain + "\" hreflang=\"x-default\" />");
            }

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString Canonical()
        {
            string canonicalUrl = string.Empty;
            var domain = SiteDefinition.Current.SiteUrl.ToString();
            var pageRouteHelper = ServiceLocator.Current.GetInstance<IPageRouteHelper>();
            var currentPage = pageRouteHelper.Page;

            if (currentPage != null)
            {
                var urlBuilder = new UrlBuilder(currentPage.LinkURL);
                canonicalUrl = WriteCanonicalLink(urlBuilder, currentPage);
                foreach (var a in SiteDefinition.Current.Hosts)
                {
                    if (a.Language != null && a.Language.Equals(currentPage.Language))
                    {
                        var siteSchema = a.UseSecureConnection.HasValue && a.UseSecureConnection.Value ? "https" : "http";
                        domain = siteSchema + "://" + a.Name;
                        break;
                    }
                }
            }

            if (domain.EndsWith("/"))
                domain = domain.Substring(0, domain.Length - 1);

            var sb = new StringBuilder();
            sb.AppendLine("<link rel=\"canonical\" href=\"" + domain + canonicalUrl + "\" />");

            return MvcHtmlString.Create(sb.ToString());
        }

        private static string WriteCanonicalLink(UrlBuilder url, PageData currentPage)
        {
            return EPiServer.Global.UrlRewriteProvider.ConvertToExternal(url, currentPage.ContentLink, Encoding.Default) ? url.Uri.ToString().ToLower() : null;
        }
    }
}