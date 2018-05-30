using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using MBran.ContentModule.Constants;
using MBran.ContentModule.Controller;
using MBran.ContentModule.Helpers;
using MBran.Core.Extensions;
using Umbraco.Core.Models;

namespace MBran.ContentModule.Extensions
{
    public static class HtmlHelperModuleExtensions
    {
        public static MvcHtmlString Module<T>(this HtmlHelper helper, 
            RouteValueDictionary routeValues = null)
            where T: class
        {
            return helper.Module<T>(string.Empty, routeValues);
        }

        public static MvcHtmlString Module<T>(this HtmlHelper helper, 
            string viewPathDirectory, 
            RouteValueDictionary routeValues = null)
            where T : class
        {
            return helper.Module<T>(null, string.Empty, viewPathDirectory, routeValues);
        }

        public static MvcHtmlString Module<T>(this HtmlHelper helper,
            IPublishedContent model,
            RouteValueDictionary routeValues = null)
            where T : class
        {
            return helper.Module<T>(model, string.Empty, routeValues);
        }

        public static MvcHtmlString Module<T>(this HtmlHelper helper, 
            IPublishedContent model,
            string action, 
            RouteValueDictionary routeValues = null)
            where T : class
        {
            return helper.Module<T>(model, action, string.Empty, routeValues);
        }

        public static MvcHtmlString Module<T>(this HtmlHelper helper, 
            IPublishedContent model,
            string action, 
            string viePathDirectory, 
            RouteValueDictionary routeValues = null)
            where T : class
        {

            var moduleType = typeof(T).AssemblyQualifiedName;
            var moduleName = typeof(T).Name;

            if (model != null)
            {
                helper.SetParentModule(model);
            }
            else
            {
                helper.SetParentModule(moduleName);
            }
            

            return helper.Module(moduleName, string.Empty, model, routeValues,
                moduleType, action, moduleType, viePathDirectory);
        }

        public static MvcHtmlString Module(this HtmlHelper helper, 
            IPublishedContent model,
            RouteValueDictionary routeValues = null)
        {
            return helper.Module(model, string.Empty, routeValues);
        }

        public static MvcHtmlString Module(this HtmlHelper helper, 
            IPublishedContent model,
            string viewPathDirectory,
            RouteValueDictionary routeValues = null)
        {
            return helper.Module(model, string.Empty, viewPathDirectory, routeValues);
        }

        public static MvcHtmlString Module(this HtmlHelper helper, 
            IPublishedContent model,
            string action, 
            string viewPathDirectory,
            RouteValueDictionary routeValues = null)
        {

            helper.SetParentModule(model);

            return helper.Module(model.GetDocumentTypeAlias(), string.Empty, model, routeValues,
                model.GetType().AssemblyQualifiedName, action, string.Empty, viewPathDirectory);
        }

        public static MvcHtmlString SubModule(this HtmlHelper helper, 
            IPublishedContent model,
            RouteValueDictionary routeValues = null)
        {
            return helper.SubModule(model, string.Empty, routeValues);
        }

        public static MvcHtmlString SubModule(this HtmlHelper helper, 
            IPublishedContent model,
            string viewPathDirectory,
            RouteValueDictionary routeValues = null)
        {
            return helper.Module(model.GetDocumentTypeAlias(), string.Empty, model, routeValues,
                model.GetType().AssemblyQualifiedName, string.Empty, string.Empty, viewPathDirectory);
        }


        public static MvcHtmlString SubModule<T>(this HtmlHelper helper, 
            IPublishedContent model,
            RouteValueDictionary routeValues = null)
            where T : class
        {
            return helper.SubModule<T>(model, string.Empty, routeValues);
        }

        public static MvcHtmlString SubModule<T>(this HtmlHelper helper, 
            IPublishedContent model,
            string viewPathDirectory,
            RouteValueDictionary routeValues = null)
            where T : class
        {
            var moduleType = typeof(T).AssemblyQualifiedName;
            var moduleName = typeof(T).Name;

            return helper.Module(moduleName, string.Empty, model, routeValues,
                moduleType, null, moduleType, viewPathDirectory);
        }

        private static MvcHtmlString Module(this HtmlHelper helper, string docType,
            string viewPath, object model,
            RouteValueDictionary routeValues,
            string contentModuleFullname, string actionName,
            string targetType, string viewPathDirectory)
        {
            var controllerName = ModuleViewHelper.Instance.GetCustomControllerName(docType);
            if (string.IsNullOrWhiteSpace(controllerName))
            {
                controllerName = ModuleViewHelper.Instance.GetDefaultControllerName();
            }

            var parentModule = helper.GetParentModule();
            var options = CreateRouteValues(docType, viewPath, model, routeValues, 
                contentModuleFullname, parentModule, targetType, viewPathDirectory);
            var action = !string.IsNullOrWhiteSpace(actionName) ? actionName : nameof(IModuleController.Index);

            return helper.Action(action, controllerName, options);
        }

        private static RouteValueDictionary CreateRouteValues(string docType,
            string viewPath, object model,
            RouteValueDictionary routeValues,
            string componentFullname, string parentModule,
            string targetType, string viewPathDirectory)
        {
            var options = routeValues ?? new RouteValueDictionary();
            options.Remove(RouteDataConstants.Model.Request);
            options.Remove(RouteDataConstants.Controller.ViewPath);
            options.Remove(RouteDataConstants.Controller.ViewPathDirectory);
            options.Remove(RouteDataConstants.Model.Fullname);
            options.Remove(RouteDataConstants.Module.Current);
            options.Remove(RouteDataConstants.Module.Parent);
            options.Remove(RouteDataConstants.Model.TargetType);
            
            options.Add(RouteDataConstants.Module.Current, docType);
            options.Add(RouteDataConstants.Module.Parent, parentModule);
            options.Add(RouteDataConstants.Model.Request, model);
            options.Add(RouteDataConstants.Controller.ViewPath, viewPath);
            options.Add(RouteDataConstants.Controller.ViewPathDirectory, viewPathDirectory);
            options.Add(RouteDataConstants.Model.Fullname, componentFullname);
            options.Add(RouteDataConstants.Model.TargetType, targetType);

            return options;
        }

        private static string GetParentModule(this HtmlHelper helper)
        {
            return helper.ViewData[ViewDataConstants.Module.Parent] as string;
        }

        private static void SetParentModule(this HtmlHelper helper, IPublishedContent model)
        {
            if (model == null) return;

            helper.SetParentModule(model.GetDocumentTypeAlias());
        }

        private static void SetParentModule(this HtmlHelper helper, string docType)
        {
            helper.ViewData[ViewDataConstants.Module.Parent] = docType;

        }
    }
}