using System;
using System.Collections.Generic;
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
            this.SetControllerAction(this.GetModuleName());
            return PartialView(GetView(), GetStronglyTypedModel());
        }


        protected virtual IEnumerable<string> GetViewPathLocations()
        {
            var moduleName = this.GetModuleName();
            var docType = CurrentPage.GetDocumentTypeAlias();

            return new List<string>
            {
                $"~/Views/{docType}/{moduleName}.cshtml",
                $"~/Views/{moduleName}/{moduleName}.cshtml"
            };
        }

        protected override PartialViewResult PartialView(string viewName, object model)
        {
            var moduleName = this.GetModuleName();
            this.SetExecutingModule(moduleName);

            var viewPath = GetView(viewName);

            return base.PartialView(!string.IsNullOrWhiteSpace(viewPath) ? viewPath : moduleName, model);
        }

        private Type GetModelType()
        {
            var cacheName = string.Join("_", GetType().FullName, nameof(GetModelType),
                this.GetModuleName(), CurrentPage.GetDocumentTypeAlias(), this.GetContentFullname());

            return (Type) ApplicationContext.Current
                .ApplicationCache
                .RuntimeCache
                .GetCacheItem(cacheName,
                    () => this.GetPocoModelType() ?? this.GetPublishedContentType() ?? this.GetPassedModelType());
        }


        private string GetView(string view = null)
        {
            var viewPath = view ?? this.GetViewPath().ToSafeAlias();
            var moduleName = this.GetModuleName();
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