using System.ComponentModel.DataAnnotations;
using Castle.Core;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Web;
using SeoBoost.Business;

namespace SeoBoost.Models.Pages
{
    [ContentType(DisplayName = "robots.txt", GUID = "97353691-05eb-4d27-b90d-79a077ad528f", Description = "Used to create editable robots.txt file.")]
    public class RobotsTxt: PageData
    {
        [Display(
            Name = "Robots.txt",
            Order = 10)]
        [UIHint(UIHint.Textarea)]
        public virtual string RobotsTxtContent { get; set; }
    }
}