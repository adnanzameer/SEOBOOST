﻿using System;
using System.Linq;
using System.Threading.Tasks;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging;
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
            try
            {
                if (!ContentReference.IsNullOrEmpty(ContentReference.StartPage))
                {
                    var content = contentRepository.GetChildren<SBRobotsTxt>(ContentReference.StartPage);

                    if (content != null)
                    {
                        var list = content.ToList();

                        if (list.Any())
                        {
                            Task.Run(async () => await SeoHelper.AddRoute());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error("SEOBOOST Initialization", ex);
            }
        }

        public void Uninitialize(InitializationEngine context) { }
    }
}