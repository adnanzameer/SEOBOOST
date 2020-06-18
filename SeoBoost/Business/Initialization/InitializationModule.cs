using System.Linq;
using System.Threading.Tasks;
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
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class InitializationModule : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var items = SeoHelper.FindPagesByPageTypeRecursively<SBRobotsTxt>(ContentReference.StartPage);
            if (items != null && items.Any())
            {
                Task.Run(async () => await SeoHelper.AddRoute());
            }
            else
            {
                var contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
                var contentType = contentTypeRepository.Load<SBRobotsTxt>();
                var writableContentType = (ContentType)contentType.CreateWritableClone();
                writableContentType.IsAvailable = true;
                contentTypeRepository.Save(writableContentType);
            }
        }

        public void Uninitialize(InitializationEngine context) { }
    }
}