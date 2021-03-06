﻿using System.Collections.Generic;

namespace SeoBoost.Models.ViewModels
{
    public class AlternativeLinkViewModel
    {
        public readonly ICollection<AlternativePageLink> Alternates;
        public string XDefaultUrl { get; set; }
        public readonly string CanonicalUrl;

        public AlternativeLinkViewModel(ICollection<AlternativePageLink> alternates)
        {
            Alternates = alternates;
            XDefaultUrl = "";
            CanonicalUrl = "";
        }
    }
}
