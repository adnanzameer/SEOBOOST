using EPiServer;
using EPiServer.Core;

namespace SeoBoost.Business.Events
{
    public class SeoBoostInitializer
    {
        private readonly IContentEvents _contentEvents;

        public SeoBoostInitializer(IContentEvents contentEvents)
        {
            _contentEvents = contentEvents;
        }
        public void Initialize()
        {

            _contentEvents.PublishingContent += Instance_PublishingPage;
            _contentEvents.MovingContent += ContentEvents_MovingContent;
            _contentEvents.DeletingContent += ContentEvents_DeletingContent;

        }

        private void Instance_PublishingPage(object sender, ContentEventArgs e)
        {
            //if (e.Content is SBRobotsTxt content)
            //{
            //    Task.Run(async () => await SeoHelper.AddRoute());
            //}
        }

        private void ContentEvents_MovingContent(object sender, ContentEventArgs e)
        {
            //if (e.Content is SBRobotsTxt)
            //{
            //    if (e.TargetLink.ID != ContentReference.WasteBasket.ID)
            //    {
            //        Task.Run(async () => await SeoHelper.AddRoute());
            //    }
            //    else
            //    {
            //        Task.Run(async () => await SeoHelper.RemoveRoute());
            //    }
            //}
        }

        private void ContentEvents_DeletingContent(object sender, ContentEventArgs e)
        {
            //if (e.Content is SBRobotsTxt)
            //{
            //    Task.Run(async () => await SeoHelper.RemoveRoute());
            //}
        }
    }
}
