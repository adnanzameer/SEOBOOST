namespace SeoBoost.Models
{
    public class AlternativePageLink
    {
        public string Url { get; set; }
        public string Culture { get; set; }

        public AlternativePageLink(string url, string culture)
        {
            Url = url.ToLower();
            Culture = culture;
        }
    }
}
