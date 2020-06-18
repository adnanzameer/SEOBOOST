using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Web;

namespace SeoBoost.Models.Pages
{
    [ContentType(DisplayName = "robots.txt", GUID = "20abf142-91eb-4a13-9196-f6de727b4e4c", GroupName = "SEO Boost", Description = "Used to create editable robots.txt file.")]
    public class SBRobotsTxt: PageData
    {
        [Display(
            Name = "Robots.txt",
            Order = 10)]
        [UIHint(UIHint.Textarea)]
        public virtual string RobotsTxtContent { get; set; }
    }
}