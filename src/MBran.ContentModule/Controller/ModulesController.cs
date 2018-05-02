using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MBran.ContentModule.Constants;
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
            SetControllerAction(GetModuleName());
            return PartialView(GetView(), GetStronglyTypedModel());
        }

        private Type GetModelType()
        {
            var cacheName = string.Join("_", GetType().FullName, nameof(GetModelType),
                GetModuleName(), CurrentPage.GetDocumentTypeAlias(), GetContentFullname());

            return (Type) ApplicationContext.Current
                .ApplicationCache
                .RuntimeCache
                .GetCacheItem(cacheName, () => GetPocoModelType() ?? GetPublishedContentType() ?? GetPassedModelType());
        }

        protected string GetModuleName()
        {
            var moduleName = GetName();
            return nameof(ModulesController).Replace("Controller", string.Empty)
                .Equals(moduleName, StringComparison.InvariantCultureIgnoreCase)
                ? GetExecutingModule()
                : moduleName;
        }

        private string GetView(string view = null)
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

        protected void SetExecutingModule(string module)
        {
            ViewData[ViewDataConstants.Module.Current] = module;
        }

        protected string GetExecutingModule()
        {
            return RouteData.Values[RouteDataConstants.Module.Current] as string;
        }

        protected string GetName()
        {
            return GetType().Name.Replace("Controller", string.Empty);
        }

        protected string GetViewPath()
        {
            return RouteData.Values[RouteDataConstants.Controller.ViewPath] as string;
        }

        protected void SetControllerAction(string action)
        {
            RouteData.Values[RouteDataConstants.Controller.Action] = action;
        }

        protected string GetControllerAction()
        {
            return RouteData.Values[RouteDataConstants.Controller.Action] as string;
        }


        protected string GetContentFullname()
        {
            return RouteData.Values[RouteDataConstants.Model.Fullname] as string;
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