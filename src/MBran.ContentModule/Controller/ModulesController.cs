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
using Umbraco.Core.Models.PublishedContent;
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
            this.SetControllerAction(GetModuleName());
            return PartialView(GetView(), GetStronglyTypedModel());
        }

        protected string GetModuleName()
        {
            var moduleName = this.GetName();
            return nameof(ModulesController).Replace("Controller", string.Empty)
                .Equals(moduleName, StringComparison.InvariantCultureIgnoreCase)
                ? this.GetExecutingModule()
                : moduleName;
        }

        protected virtual IEnumerable<string> GetViewPathLocations()
        {
            var moduleName = GetModuleName();
            var docType = CurrentPage.GetDocumentTypeAlias();

            return new List<string>
            {
                $"~/Views/{docType}/{moduleName}.cshtml",
                $"~/Views/{moduleName}/{moduleName}.cshtml"
            };
        }

        protected override PartialViewResult PartialView(string viewName, object model)
        {
            var moduleName = GetModuleName();
            this.SetExecutingModule(moduleName);

            var viewPath = GetView(viewName);

            return base.PartialView(!string.IsNullOrWhiteSpace(viewPath) ? viewPath : moduleName, model);
        }

        private Type GetModelType()
        {
            var cacheName = string.Join("_", GetType().FullName, nameof(GetModelType),
                GetModuleName(), CurrentPage.GetDocumentTypeAlias(), this.GetContentFullname());

            return (Type) ApplicationContext.Current
                .ApplicationCache
                .RuntimeCache
                .GetCacheItem(cacheName, () => GetPocoModelType() ?? GetPublishedContentType() ?? GetPassedModelType());
        }


        private string GetView(string view = null)
        {
            var viewPath = view ?? this.GetViewPath().ToSafeAlias();
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


        private Type GetPassedModelType()
        {
            var modelTypeQualifiedName = this.GetContentFullname();
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