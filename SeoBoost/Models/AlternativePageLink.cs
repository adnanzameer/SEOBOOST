namespace SeoBoost.Models
{
    public class AlternativePageLink
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Culture { get; set; }

        public AlternativePageLink(string name, string url, string culture)
        {
            this.Name = name;
            this.Url = url.ToLower();
            this.Culture = culture.ToLower();
        }
    }
}
