namespace SeoBoost.Models.ViewModels
{
    public class BreadcrumbItemListElementViewModel
    {
        public readonly string Name;
        public readonly string Url;
        public readonly int Position;
        public readonly bool Selected;
        public readonly bool VisibleInMenu;
        public readonly bool HasPageTemplate;
        public readonly bool HasChildren;

        public BreadcrumbItemListElementViewModel(string name, string url, int position, bool selected, bool visibleInMenu, bool hasPageTemplate, bool hasChildren)
        {
            Name = name;
            Url = url;
            Position = position;
            Selected = selected;
            VisibleInMenu = visibleInMenu;
            HasPageTemplate = hasPageTemplate;
            HasChildren = hasChildren;
        }
    }
}