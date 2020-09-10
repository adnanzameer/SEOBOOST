using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;
using SeoBoost.Models.Pages;

namespace SeoBoost.Controllers
{
    public class SBRobotsTxtController : Controller
    {
        public ActionResult Index()
        {
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var items = contentRepository.GetChildren<SBRobotsTxt>(ContentReference.StartPage);

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


