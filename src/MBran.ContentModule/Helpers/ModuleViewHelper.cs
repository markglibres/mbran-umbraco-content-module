using System;
using System.Linq;
using MBran.ContentModule.Controller;
using MBran.Core.Extensions;
using Umbraco.Core;

namespace MBran.ContentModule.Helpers
{
    public class ModuleViewHelper
    {
        private static readonly Lazy<ModuleViewHelper> Helper = new Lazy<ModuleViewHelper>(() => new ModuleViewHelper())
            ;

        private ModuleViewHelper()
        {
        }

        public static ModuleViewHelper Instance => Helper.Value;

        public string GetDefaultControllerName(string documentType)
        {
            var cacheName = string.Join("_", GetType().FullName, nameof(GetDefaultControllerName), documentType);
            return (string)ApplicationContext.Current
                .ApplicationCache
                .RuntimeCache
                .GetCacheItem(cacheName, () => AppDomain.CurrentDomain
                    .FindImplementation(typeof(DefaultModuleController).FullName)?.Name);
        }

        public string GetCustomControllerName(string documentType)
        {
            var cacheName = string.Join("_", GetType().FullName, nameof(GetCustomControllerName), documentType);

            return (string)ApplicationContext.Current
                .ApplicationCache
                .RuntimeCache
                .GetCacheItem(cacheName, () =>
                {
                    var docTypeController = documentType + "Controller";
                    return AppDomain.CurrentDomain
                        .FindImplementations<CustomModuleController>()
                        .FirstOrDefault(model =>
                            model.Name.Equals(docTypeController, StringComparison.InvariantCultureIgnoreCase))?.Name;
                });
        }

        public string GetActionName(string documentType)
        {
            return documentType.ToFirstUpperInvariant();
        }

    }
}