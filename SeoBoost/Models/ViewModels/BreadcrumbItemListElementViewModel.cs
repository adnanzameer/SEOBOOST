namespace SeoBoost.Models.ViewModels
{
    public class BreadcrumbItemListElementViewModel
    {
        public readonly string Name;
        public readonly string Url;
        public readonly int Position;
        public readonly bool Selected;
        public readonly bool VisibleInMenu;

        public BreadcrumbItemListElementViewModel(string name, string url, int position, bool selected, bool visibleInMenu)
        {
            Name = name;
            Url = url;
            Position = position;
            Selected = selected;
            VisibleInMenu = visibleInMenu;
        }
    }
}