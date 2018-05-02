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
        public IPublishedContent PublishedContent =>
            ControllerContext.RouteData.Values[RouteDataConstants.Model.Request] as IPublishedContent
            ?? UmbracoContext.Current.PublishedContentRequest.PublishedContent;

        public Type GetModelType()
        {
            var cacheName = string.Join("_", GetType().FullName, nameof(GetModelType),
                GetModuleName(), CurrentPage.GetDocumentTypeAlias(), GetContentFullname());

            return (Type) ApplicationContext.Current
                .ApplicationCache
                .RuntimeCache
                .GetCacheItem(cacheName, () => GetPocoModelType() ?? GetPublishedContentType() ?? GetPassedModelType() );
        }

        public string GetModuleName()
        {
            var moduleName = GetName();
            return nameof(ModulesController).Replace("Controller", string.Empty)
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

        public virtual IEnumerable<string> GetViewPathLocations()
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

        public void SetExecutingModule(string module)
        {
            ViewData[ViewDataConstants.Module.Current] = module;
        }

        public string GetExecutingModule()
        {
            return RouteData.Values[RouteDataConstants.Module.Current] as string;
        }

        public string GetName()
        {
            return GetType().Name.Replace("Controller", string.Empty);
        }

        public string GetViewPath()
        {
            return RouteData.Values[RouteDataConstants.Controller.ViewPath] as string;
        }

        public void SetControllerAction(string action)
        {
            RouteData.Values[RouteDataConstants.Controller.Action] = action;
        }

        public string GetControllerAction()
        {
            return RouteData.Values[RouteDataConstants.Controller.Action] as string;
        }


        public string GetContentFullname()
        {
            return RouteData.Values[RouteDataConstants.Model.Fullname] as string;
        }

        public virtual PartialViewResult Index()
        {
            SetControllerAction(GetModuleName());
            return PartialView(GetView(), PublishedContent.As(GetModelType()));
        }
    }
}