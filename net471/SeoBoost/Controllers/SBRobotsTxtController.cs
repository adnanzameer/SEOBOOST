using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using SeoBoost.Models.Pages;

namespace SeoBoost.Controllers
{
    public class SBRobotsTxtController : Controller
    {
        private readonly IContentLoader _contentLoader;

        public SBRobotsTxtController(IContentLoader contentLoader)
        {
            _contentLoader = contentLoader;
        }

        public ActionResult Index()
        {
            var items = _contentLoader
                .GetChildren<SBRobotsTxt>(ContentReference.StartPage, new LoaderOptions { LanguageLoaderOption.FallbackWithMaster() });

            var content = "User-agent: *" + Environment.NewLine + "Disallow: /episerver";
            if (items != null)
            {
                var robotTxtPages = items.ToList();

                if (robotTxtPages.Any())
                    content = robotTxtPages.First().RobotsContent;
            }

            return (ActionResult) this.Content(content, "text/plain", Encoding.UTF8);
        }

    }
}


