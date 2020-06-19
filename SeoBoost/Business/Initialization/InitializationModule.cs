using System.Linq;
using System.Threading.Tasks;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using SeoBoost.Helper;
using SeoBoost.Models.Pages;

namespace SeoBoost.Business.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    internal class InitializationModule : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var content = contentRepository.GetChildren<SBRobotsTxt>(ContentReference.StartPage);

            if (content == null || !content.Any())
            {
                var robotsTxtPage = contentRepository.GetDefault<SBRobotsTxt>(ContentReference.StartPage);
                robotsTxtPage.PageName = "Robots.txt";
                robotsTxtPage.VisibleInMenu = false;
                robotsTxtPage.DisableFeature = true;
                robotsTxtPage.RobotsTxtContent = "User-agent: *";
                contentRepository.Save(robotsTxtPage, EPiServer.DataAccess.SaveAction.Publish, EPiServer.Security.AccessLevel.NoAccess);
            }
            else if(!content.First().DisableFeature)
            {
                Task.Run(async () => await SeoHelper.AddRoute());
            }
        }

        public void Uninitialize(InitializationEngine context) { }
    }
}