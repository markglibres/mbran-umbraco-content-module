using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MBran.ContentModule.Constants;
using MBran.ContentModule.Extensions;
using MBran.ContentModule.Models;
using MBran.Core.Extensions;
using Our.Umbraco.Ditto;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace MBran.ContentModule.Controller
{
    public class ModulesController : SurfaceController, IModuleController
    {
        protected IPublishedContent PublishedContent =>
            ControllerContext.RouteData.Values[RouteDataConstants.Model.Request] as IPublishedContent
            ?? UmbracoContext.Current.PublishedContentRequest.PublishedContent;

        public virtual PartialViewResult Index()
        {
            return ModuleView(GetView(), GetStronglyTypedModel());
        }

        public virtual PartialViewResult ModuleView(string viewName, object model)
        {
            this.SetControllerAction(this.GetModuleName());
            var moduleName = this.GetModuleName();
            this.SetExecutingModule(moduleName);

            var viewPath = GetView(viewName);
            return PartialView(!string.IsNullOrWhiteSpace(viewPath) ? viewPath : moduleName, model);
        }

        public virtual PartialViewResult ModuleView(object model)
        {
            return ModuleView(GetView(), model);
        }


        protected virtual IEnumerable<string> GetViewPathLocations()
        {
            var moduleName = this.GetModuleName();
            var docType = CurrentPage.GetDocumentTypeAlias();

            var parent = this.GetParentModule();

            var parentLocations = new List<string>
            {
                $"~/Views/{docType}/{parent}/{moduleName}.cshtml",
                $"~/Views/{parent}/{moduleName}.cshtml"
            };

            var defaultLocations = new List<string>
            {
                $"~/Views/{docType}/{moduleName}.cshtml",
                $"~/Views/{moduleName}/{moduleName}.cshtml",
                $"~/Views/Modules/{moduleName}.cshtml"
            };

            if (string.IsNullOrWhiteSpace(parent))
                return defaultLocations;

            parentLocations.AddRange(defaultLocations);
            return parentLocations.Distinct();
        }

        private Type GetModelType()
        {
            var cacheName = string.Join("_", GetType().FullName, nameof(GetModelType),
                this.GetModuleName(), CurrentPage.GetDocumentTypeAlias(), this.GetContentFullname());

            return (Type) ApplicationContext.Current
                .ApplicationCache
                .RuntimeCache
                .GetCacheItem(cacheName,
                    () => this.GetPassedModelType() ?? this.GetPocoModelType() ?? this.GetPublishedContentType() ?? typeof(IPublishedContent) );
        }


        private string GetView(string view = null)
        {
            var viewPath = view ?? this.GetViewPath().ToSafeAlias();
            var moduleName = this.GetModuleName();
            var parentModule = this.GetParentModule();
            var cacheName = string.Join("_", GetType().FullName, nameof(GetView),
                moduleName, parentModule, CurrentPage.GetDocumentTypeAlias(), viewPath);

            return (string) ApplicationContext.Current
                .ApplicationCache
                .RuntimeCache
                .GetCacheItem(cacheName, () => GetViewPath(viewPath));
        }

        private string GetViewPath(string viewPath)
        {
            if (!string.IsNullOrWhiteSpace(viewPath) && this.PartialViewExists(viewPath)) return viewPath;

            var viewLocations = GetViewPathLocations();
            foreach (var viewLocation in viewLocations)
                if (this.PartialViewExists(viewLocation))
                    return viewLocation;

            return string.Empty;
        }

        protected object GetStronglyTypedModel()
        {
            var modelType = GetModelType();
            var model = PublishedContent.As(modelType);
            IModuleModel module;
            if ((module = model as IModuleModel) == null)
                return model;
            module.Content = PublishedContent;
            return module;
        }
    }
}