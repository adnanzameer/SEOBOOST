using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using SeoBoost.Models.Pages;

namespace SeoBoost.Business.Initialization
{
    [ModuleDependency(typeof(InitializationModule))]
    internal class CustomizedRenderingInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            context.Locate.TemplateResolver()
                .TemplateResolved += TemplateCoordinator.OnTemplateResolved;
        }

        public void Uninitialize(InitializationEngine context)
        {
            ServiceLocator.Current.GetInstance<TemplateResolver>()
                .TemplateResolved -= TemplateCoordinator.OnTemplateResolved;
        }

        public void Preload(string[] parameters)
        {
        }
    }

    [ServiceConfiguration(typeof(IViewTemplateModelRegistrator))]
    internal class TemplateCoordinator : IViewTemplateModelRegistrator
    {
        public static void OnTemplateResolved(object sender, TemplateResolverEventArgs args)
        {
            if (args.ItemToRender is SBRobotsTxt)
            {
                args.SelectedTemplate = null;
            }
        }

        public void Register(TemplateModelCollection viewTemplateModelRegistrator)
        {

        }

    }
}
