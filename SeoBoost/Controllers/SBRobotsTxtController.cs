using System.Linq;
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
        [ContentOutputCache]
        public ActionResult Index()
        {
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var items = contentRepository.GetChildren<SBRobotsTxt>(ContentReference.StartPage);

            var content = "";
            if (items != null && items.Any())
            {
                content = items.First().RobotsTxtContent;
            }

            return Content(content, "text/plain");
        }
    }
}


