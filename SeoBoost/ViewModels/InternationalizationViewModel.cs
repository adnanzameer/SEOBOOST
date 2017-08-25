using System.Collections.Generic;
using SeoBoost.Models;

namespace SeoBoost.ViewModels
{
    public class InternationalizationViewModel
    {
        public ICollection<AlternativePageLink> Alternates { get; set; }
        public string XDefaultUrl { get; set; }
        public string CanonicalUrl { get; set; }

        public InternationalizationViewModel(ICollection<AlternativePageLink> alternates)
        {
            this.Alternates = alternates;
            XDefaultUrl = "";
            CanonicalUrl = "";
        }
    }
}
