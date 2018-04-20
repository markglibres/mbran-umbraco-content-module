using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MBran.ContentModule.Models;
using MBran.Core.Extensions;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;

namespace MBran.ContentModule.Controller
{
    public abstract class ModuleController : BaseContentModuleController, IModuleController
    {
        public Type GetModelType()
        {
            var cacheName = string.Join("_", GetType().FullName, nameof(GetModelType),
                GetModuleName(), CurrentPage.GetDocumentTypeAlias(), GetContentFullname());

            return (Type)ApplicationContext.Current
                .ApplicationCache
                .RuntimeCache
                .GetCacheItem(cacheName, () => GetPassedModelType() ?? GetPublishedContentType() ?? GetPocoModelType());
        }

        public string GetModuleName()
        {
            var moduleName = GetName();
            return nameof(DefaultModuleController).Replace("Controller", string.Empty)
                .Equals(moduleName, StringComparison.InvariantCultureIgnoreCase)
                ? GetExecutingModule()
                : moduleName;
        }

        public string GetView(string view = null)
        {
            var viewPath = view ?? GetViewPath().ToSafeAlias();
            var moduleName = GetModuleName();
            var cacheName = string.Join("_", GetType().FullName, nameof(GetView),
                moduleName, CurrentPage.GetDocumentTypeAlias(), viewPath);

            return (string) ApplicationContext.Current
                .ApplicationCache
                .RuntimeCache
                .GetCacheItem(cacheName, () =>
                {
                    if (!string.IsNullOrWhiteSpace(viewPath) && this.PartialViewExists(viewPath)) return viewPath;

                    var viewLocations = GetViewPathLocations();
                    foreach (var viewLocation in viewLocations)
                        if (this.PartialViewExists(viewLocation))
                            return viewLocation;

                    return string.Empty;
                });
        }

        public IEnumerable<string> GetViewPathLocations()
        {
            var moduleName = GetModuleName();
            var docType = CurrentPage.GetDocumentTypeAlias();

            return new List<string>
            {
                $"~/Views/{docType}/{moduleName}/{moduleName}.cshtml",
                $"~/Views/Modules/{moduleName}/{moduleName}.cshtml"
            };
        }

        protected override PartialViewResult PartialView(string viewName, object model)
        {
            var moduleName = GetModuleName();
            SetExecutingModule(moduleName);
            
            var viewPath = GetView(viewName);

            return base.PartialView(!string.IsNullOrWhiteSpace(viewPath) ? viewPath : moduleName, model);
        }

        private Type GetPassedModelType()
        {
            var modelTypeQualifiedName = GetContentFullname();
            return string.IsNullOrWhiteSpace(modelTypeQualifiedName) ? null : Type.GetType(modelTypeQualifiedName);
        }

        private Type GetPublishedContentType()
        {
            return AppDomain.CurrentDomain.FindImplementations<PublishedContentModel>()
                .FirstOrDefault(type =>
                    type.Name.Equals(GetModuleName(), StringComparison.InvariantCultureIgnoreCase));
        }

        private Type GetPocoModelType()
        {
            return AppDomain.CurrentDomain.FindImplementations<IModuleModel>()
                .FirstOrDefault(type =>
                    type.Name.Equals(GetModuleName(), StringComparison.InvariantCultureIgnoreCase));
        }
    }
}