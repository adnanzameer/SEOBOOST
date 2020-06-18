using System.Linq;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Web.Mvc;
using SeoBoost.Helper;
using SeoBoost.Models.Pages;

namespace SeoBoost.Controllers
{
    public class RobotsTxtController : Controller
    {
        //[ContentOutputCache]
        public ActionResult Index()
        {
            var content = "";
            var items = SeoHelper.FindPagesByPageTypeRecursively<RobotsTxt>(ContentReference.StartPage);
            if (items != null && items.Any())
            {
                content = items.First().RobotsTxtContent;
            }

            return Content(content, "text/plain");
        }
    }
}


