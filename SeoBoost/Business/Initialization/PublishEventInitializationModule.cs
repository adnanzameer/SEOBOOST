using System.Linq;
using System.Threading.Tasks;
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
            contentEvents.DeletingContent += Instance_DeletingContent;
            contentEvents.MovingContent += ContentEvents_MovingContent;
        }

        public void Uninitialize(InitializationEngine context)
        {
            var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();
            contentEvents.PublishingContent -= Instance_PublishingPage;
            contentEvents.CreatedContent -= Instance_CreatedContent;
            contentEvents.DeletingContent -= Instance_DeletingContent;
            contentEvents.MovingContent -= ContentEvents_MovingContent;
        }

        private void Instance_PublishingPage(object sender, ContentEventArgs e)
        {
            if (e.Content is SBRobotsTxt)
            {
                Task.Run(async () => await SeoHelper.AddRoute());
            }
        }

        private void Instance_CreatedContent(object sender, ContentEventArgs e)
        {
            var repository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();

            if (repository != null)
            {
                var contentType = repository.Load(e.Content.ContentTypeID);

                if (e.Content is SBRobotsTxt)
                {
                    var writableContentType = (ContentType)contentType.CreateWritableClone();
                    writableContentType.IsAvailable = false;
                    repository.Save(writableContentType);
                }
            }
        }

        private void ContentEvents_MovingContent(object sender, ContentEventArgs e)
        {
            if (e.Content is SBRobotsTxt)
            {
                if (e.TargetLink.ID == 2)
                {
                    Task.Run(async () => await SeoHelper.RemoveRoute());
                        
                }
                else if (((EPiServer.MoveContentEventArgs)e).OriginalParent.ID == 2)
                {
                    
                    Task.Run(async () => await SeoHelper.AddRoute());
                }
            }
        }

        private void Instance_DeletingContent(object sender, DeleteContentEventArgs e)
        {
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();

            if (e.DeletedDescendents != null && e.DeletedDescendents.Any())
            {
                foreach (var cr in e.DeletedDescendents)
                {
                    contentRepository.TryGet(cr, out SBRobotsTxt content);

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
            else
            {
                contentRepository.TryGet(e.ContentLink, out SBRobotsTxt content);
                if (content != null)
                {
                    var contentType = contentTypeRepository.Load(content.ContentTypeID);
                    var writableContentType = (ContentType)contentType.CreateWritableClone();
                    writableContentType.IsAvailable = true;
                    contentTypeRepository.Save(writableContentType);
                }
            }

        }
    }
}

