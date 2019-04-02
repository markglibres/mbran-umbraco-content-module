using System;
using System.Collections.Generic;
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

        public string GetDefaultControllerName()
        {
#if DEBUG
            return GetDefaultControllerNameFromAssembly();
#else
            var cacheName = string.Join("_", GetType().FullName, nameof(GetDefaultControllerName));
            return (string)ApplicationContext.Current
                .ApplicationCache
                .RuntimeCache
                .GetCacheItem(cacheName, GetDefaultControllerNameFromAssembly);
#endif

        }

        private string GetDefaultControllerNameFromAssembly()
        {
            var cacheName = string.Join("_", typeof(AssemblyExtensions).FullName, nameof(GetDefaultControllerNameFromAssembly),
                nameof(Type), typeof(ModulesController));

            var defaultControllerName = (string)ApplicationContext.Current
                .ApplicationCache
                .RuntimeCache
                .GetCacheItem(cacheName, () =>
                {
                    return AppDomain.CurrentDomain
                        .FindImplementations<ModulesController>()
                        .FirstOrDefault(model => model.IsSubclassOf(typeof(ModulesController))
                            && model.Name.EndsWith(nameof(ModulesController), StringComparison.InvariantCultureIgnoreCase))?.Name;
                });

            if (string.IsNullOrWhiteSpace(defaultControllerName)) 
                defaultControllerName = AppDomain.CurrentDomain
                    .FindImplementation(typeof(ModulesController).FullName)
                    ?.Name;

            return defaultControllerName?.Replace("Controller", string.Empty);
        }

        public string GetCustomControllerName(string documentType)
        {
#if DEBUG
            return GetCustomControllerNameFromAssembly(documentType);
#else
            var cacheName = string.Join("_", GetType().FullName, nameof(GetCustomControllerName), documentType);

            return (string)ApplicationContext.Current
                .ApplicationCache
                .RuntimeCache
                .GetCacheItem(cacheName, () => GetCustomControllerNameFromAssembly(documentType));
#endif

        }

        private string GetCustomControllerNameFromAssembly(string documentType)
        {
            var docTypeController = documentType + "Controller";
            return AppDomain.CurrentDomain
                .FindImplementations<ModulesController>()
                .FirstOrDefault(model =>
                    model.Name.Equals(docTypeController, StringComparison.InvariantCultureIgnoreCase))
                ?.Name.Replace("Controller", string.Empty);
        }

        public string GetActionName(string documentType)
        {
            return documentType.ToFirstUpperInvariant();
        }

    }
}