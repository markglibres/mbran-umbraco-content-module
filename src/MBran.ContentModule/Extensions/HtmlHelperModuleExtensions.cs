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
        public static MvcHtmlString Module(this HtmlHelper helper, IPublishedContent model,
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
            string contentModuleFullname = null, string actionName = null)
        {
            var controllerName = ModuleViewHelper.Instance.GetCustomControllerName(docType);
            if (string.IsNullOrWhiteSpace(controllerName))
            {
                controllerName = ModuleViewHelper.Instance.GetDefaultControllerName();
            }

            var options = CreateRouteValues(docType, viewPath, model, routeValues, contentModuleFullname);
            var action = actionName ?? nameof(IModuleController.Index);

            return helper.Action(action, controllerName, options);
        }

        private static RouteValueDictionary CreateRouteValues(string docType,
            string viewPath, object model,
            RouteValueDictionary routeValues = null,
            string componentFullname = null)
        {
            var options = routeValues ?? new RouteValueDictionary();
            options.Remove(RouteDataConstants.Model.Request);
            options.Remove(RouteDataConstants.Controller.ViewPath);
            options.Remove(RouteDataConstants.Model.Fullname);
            options.Remove(RouteDataConstants.Module.Current);

            options.Add(RouteDataConstants.Module.Current, docType);
            options.Add(RouteDataConstants.Model.Request, model);
            options.Add(RouteDataConstants.Controller.ViewPath, viewPath);
            options.Add(RouteDataConstants.Model.Fullname, componentFullname);

            return options;
        }
    }
}