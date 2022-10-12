using System;
using System.Linq;
using System.Text;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using SeoBoost.Models.Pages;

namespace SeoBoost.Controllers
{
    public class SBRobotsTxtController : PageController<SBRobotsTxt>
    {
        private readonly IContentLoader _contentLoader;

        public SBRobotsTxtController(IContentLoader contentLoader)
        {
            _contentLoader = contentLoader;
        }

        [Route("robots.txt")]
        public IActionResult Index()
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

            return Content(content, "text/plain", Encoding.UTF8);
        }

    }
}


