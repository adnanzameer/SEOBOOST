using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
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
            contentEvents.CreatedContent += Instance_CreatedContent;
            contentEvents.DeletedContent += Instance_DeletedContent;
            contentEvents.MovingContent += ContentEvents_MovingContent;
        }

        public void Uninitialize(InitializationEngine context)
        {
            var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();
            contentEvents.PublishingContent -= Instance_PublishingPage;
            contentEvents.CreatedContent -= Instance_CreatedContent;
            contentEvents.DeletedContent -= Instance_DeletedContent;
            contentEvents.MovingContent -= ContentEvents_MovingContent;
        }

        private void Instance_PublishingPage(object sender, ContentEventArgs e)
        {
            if (e.Content is RobotsTxt)
            {
                SeoHelper.AddRoute();
            }
        }

        private void Instance_CreatedContent(object sender, ContentEventArgs e)
        {
            var repository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();

            if (repository != null)
            {
                var contentType = repository.Load(e.Content.ContentTypeID);

                if (e.Content is RobotsTxt)
                {
                    var writableContentType = (ContentType)contentType.CreateWritableClone();
                    writableContentType.IsAvailable = false;
                    repository.Save(writableContentType);
                }
            }
        }

        private void ContentEvents_MovingContent(object sender, ContentEventArgs e)
        {
            if (e.Content is RobotsTxt)
            {
                if (e.TargetLink.ID == 2)
                {
                    SeoHelper.RemoveRoute();
                }
                else if (((EPiServer.MoveContentEventArgs)e).OriginalParent.ID == 2)
                {
                    SeoHelper.AddRoute();
                }
            }
        }

        private void Instance_DeletedContent(object sender, DeleteContentEventArgs e)
        {
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();

            var proceed = true;
            if (e.Content is RobotsTxt)
            {
                var contentType = contentTypeRepository.Load(e.Content.ContentTypeID);
                var writableContentType = (ContentType)contentType.CreateWritableClone();
                writableContentType.IsAvailable = true;
                contentTypeRepository.Save(writableContentType);
                proceed = false;
            }

            if (proceed && e.DeletedDescendents != null && e.DeletedDescendents.Any())
            {
                foreach (var cr in e.DeletedDescendents)
                {
                    var content = contentRepository.Get<IContent>(cr);

                    if (content != null)
                    {
                        var contentType = contentTypeRepository.Load(content.ContentTypeID);

                        if (contentType != null)
                        {
                            var writableContentType = (ContentType)contentType.CreateWritableClone();
                            writableContentType.IsAvailable = true;
                            contentTypeRepository.Save(writableContentType);
                        }

                    }
                }
            }

        }
    }
}

