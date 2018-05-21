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
        public static MvcHtmlString Module<T>(this HtmlHelper helper, RouteValueDictionary routeValues = null)
            where T: class
        {
            var moduleName = typeof(T).Name;
            var moduleType = typeof(T).AssemblyQualifiedName;
            helper.SetParentModule(moduleName);
            return helper.Module(moduleName, string.Empty, null, routeValues,
                moduleType, null, moduleType);
        }
        public static MvcHtmlString Module(this HtmlHelper helper, IPublishedContent model,
            RouteValueDictionary routeValues = null)
        {
            var docType = model.GetDocumentTypeAlias();
            helper.SetParentModule(docType);
            return helper.Module(docType, string.Empty, model, routeValues,
                model.GetType().AssemblyQualifiedName);
        }

        public static MvcHtmlString SubModule(this HtmlHelper helper, IPublishedContent model,
            RouteValueDictionary routeValues = null)
        {
            return helper.Module(model.GetDocumentTypeAlias(), string.Empty, model, routeValues,
                model.GetType().AssemblyQualifiedName);
        }

        public static MvcHtmlString Module(this HtmlHelper helper, IPublishedContent model, 
            string action,
            RouteValueDictionary routeValues = null)
        {
            return helper.Module(model.GetDocumentTypeAlias(), string.Empty, model, routeValues,
                model.GetType().AssemblyQualifiedName, action);
        }

        private static MvcHtmlString Module(this HtmlHelper helper, string docType,
            string viewPath, object model,
            RouteValueDictionary routeValues = null,
            string contentModuleFullname = null, string actionName = null,
            string targetType = null)
        {
            var controllerName = ModuleViewHelper.Instance.GetCustomControllerName(docType);
            if (string.IsNullOrWhiteSpace(controllerName))
            {
                controllerName = ModuleViewHelper.Instance.GetDefaultControllerName();
            }

            var parentModule = helper.GetParentModule();
            var options = CreateRouteValues(docType, viewPath, model, routeValues, 
                contentModuleFullname, parentModule, targetType);
            var action = actionName ?? nameof(IModuleController.Index);

            return helper.Action(action, controllerName, options);
        }

        private static RouteValueDictionary CreateRouteValues(string docType,
            string viewPath, object model,
            RouteValueDictionary routeValues = null,
            string componentFullname = null, string parentModule = null,
            string targetType = null)
        {
            var options = routeValues ?? new RouteValueDictionary();
            options.Remove(RouteDataConstants.Model.Request);
            options.Remove(RouteDataConstants.Controller.ViewPath);
            options.Remove(RouteDataConstants.Model.Fullname);
            options.Remove(RouteDataConstants.Module.Current);
            options.Remove(RouteDataConstants.Module.Parent);
            options.Remove(RouteDataConstants.Model.TargetType);

            options.Add(RouteDataConstants.Module.Current, docType);
            options.Add(RouteDataConstants.Module.Parent, parentModule);
            options.Add(RouteDataConstants.Model.Request, model);
            options.Add(RouteDataConstants.Controller.ViewPath, viewPath);
            options.Add(RouteDataConstants.Model.Fullname, componentFullname);
            options.Add(RouteDataConstants.Model.TargetType, targetType);

            return options;
        }

        private static string GetParentModule(this HtmlHelper helper)
        {
            return helper.ViewData[ViewDataConstants.Module.Parent] as string;
        }

        private static void SetParentModule(this HtmlHelper helper, string docType)
        {
            helper.ViewData[ViewDataConstants.Module.Parent] = docType;
        }
    }
}