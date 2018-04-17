using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBran.ContentModule.Controller;
using MBran.Core.Extensions;
using Umbraco.Core;

namespace MBran.ContentModule.Helpers
{
    public class ContentModuleViewHelper
    {
        private static readonly Lazy<ContentModuleViewHelper> Helper = new Lazy<ContentModuleViewHelper>(() => new ContentModuleViewHelper());
        public static ContentModuleViewHelper Instance => Helper.Value;
        private ContentModuleViewHelper()
        {
            
        }

        public string GetControllerName(string documentType)
        {
            var componentController = FindController(documentType);
            return componentController != null 
                ? documentType 
                : nameof(ContentModuleController).Replace("Controller", string.Empty);
        }

        public string GetActionName(string documentType)
        {
            return documentType.ToFirstUpperInvariant();
        }

        public Type FindController(string docTypeAlias)
        {
            var cacheName = string.Join("_", GetType().FullName, nameof(FindController), docTypeAlias);

            return (Type)ApplicationContext.Current
                .ApplicationCache
                .RuntimeCache
                .GetCacheItem(cacheName, () =>
                {
                    var docTypeController = docTypeAlias + "Controller";
                    return AppDomain.CurrentDomain
                        .FindImplementations<IContentModule>()
                        .FirstOrDefault(model =>
                            model.Name.Equals(docTypeController, StringComparison.InvariantCultureIgnoreCase));
                });
        }
    }
}
