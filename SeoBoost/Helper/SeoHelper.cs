using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using SeoBoost.Business.Url;
using SeoBoost.Models;
using SeoBoost.ViewModels;

namespace SeoBoost.Helper
{
    public static class SeoHelper
    {
        public static MvcHtmlString GetAlternateLinks(this HtmlHelper html)
        {
            var alternates = new List<AlternativePageLink>();
        
            var languages = ServiceLocator.Current.GetInstance<ILanguageBranchRepository>().ListEnabled();

            var requestContext = html.ViewContext.RequestContext;
            var contentReference = requestContext.GetContentLink();
            var domain = GetDomain(contentReference);

            GetAlternativePageLink(contentReference, languages, domain, alternates);

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
                    var fallbackLanguages = ContentLanguageSettingsHandler.Instance.GetFallbackLanguages(contentReference, language);
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
            if (xDefault != null && !string.IsNullOrEmpty(xDefault.Url))
                model.XDefaultUrl = xDefault.Url;
            var htmlString = CreateHtmlString(model);

            return htmlString;
        }

        private static string GetDomain(ContentReference contentReference)
        {
            var urlService = ServiceLocator.Current.GetInstance<IUrlService>();
            return urlService.GetHost(contentReference);
        }

        private static void GetAlternativePageLink(ContentReference contentReference, IList<LanguageBranch> languages, string domain, List<AlternativePageLink> alternates)
        {
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var pageLanguages = contentRepository.GetLanguageBranches<PageData>(contentReference);
            var urlService = ServiceLocator.Current.GetInstance<IUrlService>();

            var pagesData = pageLanguages as IList<PageData> ?? pageLanguages.ToList();
            foreach (var language in languages)
            {
                foreach (var p in pagesData)
                {
                    if (string.Equals(p.LanguageBranch.ToLower(), language.LanguageID.ToLower(),
                        StringComparison.Ordinal))
                    {
                        var url = urlService.GetExternalFriendlyUrl(contentReference, p.LanguageBranch);
                        var alternate =new AlternativePageLink(url, language.LanguageID);

                        alternates.Add(alternate);
                        break;
                    }
                }
            }
        }

        private static MvcHtmlString CreateHtmlString(InternationalizationViewModel model)
        {
            var sb = new StringBuilder();

            foreach (var alternate in model.Alternates)
            {
                sb.AppendLine("<link rel=\"alternate\" href=\"" + alternate.Url + "\" hreflang=\"" + alternate.Culture + "\" />");
            }

            if (!string.IsNullOrEmpty(model.XDefaultUrl))
                sb.AppendLine(" <link rel=\"alternate\" href=\"" + model.XDefaultUrl + "\" hreflang=\"x-default\" />");

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString GetCanonicalLink(this HtmlHelper html)
        {
            var requestContext = html.ViewContext.RequestContext;
            var contentReference = requestContext.GetContentLink();
            var urlService = ServiceLocator.Current.GetInstance<IUrlService>();

            var sb = new StringBuilder();
            sb.AppendLine("<link rel=\"canonical\" href=\"" + urlService.GetExternalFriendlyUrl(contentReference) + "\" />");
            return MvcHtmlString.Create(sb.ToString());
        }
    }
}