using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Umbraco.Core.Models;
using System.Web.Routing;
using MBran.ContentModule.Constants;
using MBran.ContentModule.Helpers;
using MBran.Core.Extensions;

namespace MBran.ContentModule.Extensions
{
    public static class HtmlHelperComponentExtensions
    {
        public static MvcHtmlString Component(this HtmlHelper helper, IPublishedContent model,
            RouteValueDictionary routeValues = null)
        {
            return helper.Component(model.GetDocumentTypeAlias(), string.Empty, model, routeValues,
                model.GetType().AssemblyQualifiedName);
        }

        private static MvcHtmlString Component(this HtmlHelper helper, string docType,
            string viewPath, object model,
            RouteValueDictionary routeValues = null,
            string contentModuleFullname = null)
        {
            var controllerName = ContentModuleViewHelper.Instance.GetControllerName(docType);
            var options = helper.CreateRouteValues(docType, viewPath, model, routeValues, contentModuleFullname);

            return helper.Action(ContentModuleViewHelper.Instance.GetActionName(docType), controllerName, options);
        }

        private static RouteValueDictionary CreateRouteValues(this HtmlHelper helper, string docType,
            string viewPath, object model,
            RouteValueDictionary routeValues = null,
            string componentFullname = null)
        {
            var options = routeValues ?? new RouteValueDictionary();
            options.Remove(RouteDataConstants.ModelKey);
            options.Remove(RouteDataConstants.ViewPathKey);
            options.Remove(RouteDataConstants.ModelFullnameKey);
            options.Remove(RouteDataConstants.ModuleNameKey); 


            options.Add(RouteDataConstants.ModuleNameKey, docType);
            options.Add(RouteDataConstants.ModelKey, model);
            options.Add(RouteDataConstants.ViewPathKey, viewPath);
            options.Add(RouteDataConstants.ModelFullnameKey, componentFullname);

            return options;
        }
    }
}
