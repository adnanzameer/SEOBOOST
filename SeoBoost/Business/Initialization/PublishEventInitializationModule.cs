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
    internal class SavePublishEventInitializationModule : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();
            contentEvents.PublishingContent += Instance_PublishingPage;
            contentEvents.MovingContent += ContentEvents_MovingContent;
            contentEvents.DeletingContent += ContentEvents_DeletingContent;
        }

        public void Uninitialize(InitializationEngine context)
        {
            var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();
            contentEvents.PublishingContent -= Instance_PublishingPage;
            contentEvents.MovingContent -= ContentEvents_MovingContent;
        }

        private void Instance_PublishingPage(object sender, ContentEventArgs e)
        {
            if (e.Content is SBRobotsTxt content)
            {
                Task.Run(async () => await SeoHelper.AddRoute());
            }
        }

        private void ContentEvents_MovingContent(object sender, ContentEventArgs e)
        {
            if (e.Content is SBRobotsTxt)
            {
                if (e.TargetLink.ID != ContentReference.WasteBasket.ID)
                {
                    Task.Run(async () => await SeoHelper.AddRoute());
                }
                else
                {
                    Task.Run(async () => await SeoHelper.RemoveRoute());
                }
            }
        }

        private void ContentEvents_DeletingContent(object sender, ContentEventArgs e)
        {
            if (e.Content is SBRobotsTxt)
            {
                Task.Run(async () => await SeoHelper.RemoveRoute());
            }
        }
    }
}

