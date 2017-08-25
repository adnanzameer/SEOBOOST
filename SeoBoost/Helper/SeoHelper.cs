using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using SeoBoost.Models;
using SeoBoost.ViewModels;

namespace SeoBoost.Helper
{
    public static class SeoHelper
    {
        public static MvcHtmlString Internationalization(this PageData page)
        {
            var alternates = new List<AlternativePageLink>();
            var domain = GetDomain();

            var languages = ServiceLocator.Current.GetInstance<ILanguageBranchRepository>().ListEnabled();

            GetAlternativePageLink(page.PageLink, languages, domain, alternates);

            var masterLanguage = languages.FirstOrDefault(l => l.ID == 1) ?? languages.FirstOrDefault();

            var xDefault = alternates.FirstOrDefault(a => string.Equals(a.Culture.ToLower(), masterLanguage.LanguageID.ToLower()));

            var siteLanguageList = languages.Select(language => language.LanguageID).ToList();

            #region For fallback language page links 

            foreach (var link in alternates)
            {
                if (siteLanguageList.Contains(link.Culture))
                {
                    siteLanguageList.Remove(link.Culture);
                }
            }

            foreach (var language in siteLanguageList)
            {
                if (xDefault != null && language != null)
                {
                    var fallbackLanguages = ContentLanguageSettingsHandler.Instance.GetFallbackLanguages(page.PageLink, language);
                    if (fallbackLanguages.Any() && fallbackLanguages.Contains(masterLanguage.LanguageID))
                    {
                        var url = xDefault.Url.Replace("/" + masterLanguage.LanguageID.ToLower() + "/", "/" + language + "/");
                        var alternate = new AlternativePageLink(url, language);
                        alternates.Add(alternate);
                    }
                }
            }

            #endregion

            var model = new InternationalizationViewModel(alternates);
            ManageLinks(model, alternates, xDefault);
            var htmlString = CreateHtmlString(model, domain);

            return htmlString;
        }

        private static string GetDomain()
        {
            var domain = SiteDefinition.Current.SiteUrl.ToString();

            var hostDefinition =
                SiteDefinition.Current.Hosts.FirstOrDefault(a => a.Type.Equals(HostDefinitionType.Primary));

            if (hostDefinition != null)
                domain = SiteDefinition.Current.SiteUrl.Scheme + "://" + hostDefinition.Name;

            if (domain.EndsWith("/"))
                domain = domain.Substring(0, domain.Length - 1);
            return domain;
        }

        private static void GetAlternativePageLink(PageReference currentPage, IList<LanguageBranch> languages, string domain, List<AlternativePageLink> alternates)
        {
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var pageLanguages = contentRepository.GetLanguageBranches<PageData>(currentPage);

            foreach (var language in languages)
            {
                foreach (var p in pageLanguages)
                {
                    if (string.Equals(p.LanguageBranch.ToLower(), language.LanguageID.ToLower(),
                        StringComparison.Ordinal))
                    {
                        var url = GenericFunctions.GetFriendlyUrl(currentPage, p.LanguageBranch);
                        var host = domain;
                        if (!string.IsNullOrEmpty(url) && url.ToLower().Contains("//"))
                        {
                            var path = new Uri(url).AbsolutePath;
                            url = "/" + language.LanguageID + path;
                        }
                        else if (string.Equals(url, "/"))
                            url = "/" + language.LanguageID + "/";
                        else if (!url.StartsWith("/" + language.LanguageID.ToLower() + "/"))
                            url = "/" + language.LanguageID + url;

                        var alternate =
                            new AlternativePageLink(string.Format("{0}{1}", host, url), language.LanguageID);

                        alternates.Add(alternate);
                        break;
                    }
                }
            }
        }

        private static void ManageLinks(InternationalizationViewModel model, List<AlternativePageLink> alternates, AlternativePageLink xDefault)
        {
            var currentLanguageBranch = ContentLanguage.PreferredCulture.Name;
            var canonical = alternates.FirstOrDefault(a => string.Equals(a.Culture.ToLower(), currentLanguageBranch.ToLower()));

            if (canonical != null && !string.IsNullOrEmpty(canonical.Url))
                model.CanonicalUrl = canonical.Url;

            if (xDefault != null && !string.IsNullOrEmpty(xDefault.Url))
                model.XDefaultUrl = xDefault.Url;
        }

        private static MvcHtmlString CreateHtmlString(InternationalizationViewModel model, string domain)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(model.CanonicalUrl))
                sb.AppendLine("<link rel=\"canonical\" href=\"" + domain + model.CanonicalUrl + "\" />");

            foreach (var alternate in model.Alternates)
            {
                sb.AppendLine("<link rel=\"alternate\" href=\"" + alternate.Url + "\" hreflang=\"" + alternate.Culture + "\" />");
            }

            if (!string.IsNullOrEmpty(model.XDefaultUrl))
                sb.AppendLine(" <link rel=\"alternate\" href=\"" + model.XDefaultUrl + "\" hreflang=\"x-default\" />");

            return MvcHtmlString.Create(sb.ToString());
        }

    }
}