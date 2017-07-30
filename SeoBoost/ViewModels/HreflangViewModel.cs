using System.Collections.Generic;
using SeoBoost.Models;

namespace SeoBoost.ViewModels
{
    public class HrefLangViewModel
    {
        public bool IsStartPage { get; set; }
        public string Domain { get; set; }
        public ICollection<AlternativePageLink> Alternates { get; set; }

        public HrefLangViewModel(bool isStartPage, string domain, ICollection<AlternativePageLink> alternates)
        {
            this.IsStartPage = isStartPage;
            this.Domain = domain;
            this.Alternates = alternates;
        }
    }
}
