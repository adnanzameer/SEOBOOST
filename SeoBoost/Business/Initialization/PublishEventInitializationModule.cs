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
    public class SavePublishEventInitializationModule : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();
            contentEvents.PublishingContent += Instance_PublishingPage;
            contentEvents.MovingContent += ContentEvents_MovingContent;
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
                if (content.DisableFeature)
                {
                    Task.Run(async () => await SeoHelper.RemoveRoute());
                }
            }
        }

        private void ContentEvents_MovingContent(object sender, ContentEventArgs e)
        {
            if (e.Content is SBRobotsTxt && e.TargetLink.ID != ContentReference.StartPage.ID)
            {
                var action = e.TargetLink == ContentReference.WasteBasket ? "remove" : "move";
                e.CancelAction = true;
                e.CancelReason = $"You can't {action} the page. This page is a part of SEOBOOST package. Manage robot.txt settings trough the page properties.";
            }
        }
    }
}

