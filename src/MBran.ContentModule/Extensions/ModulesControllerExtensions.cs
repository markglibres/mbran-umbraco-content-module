using System;
using System.Linq;
using System.Web.SessionState;
using MBran.ContentModule.Constants;
using MBran.ContentModule.Controller;
using MBran.ContentModule.Models;
using MBran.Core.Extensions;
using Umbraco.Core.Models.PublishedContent;

namespace MBran.ContentModule.Extensions
{
    public static class ModulesControllerExtensions
    {
        public static void SetExecutingModule(this ModulesController controller, string module)
        {
            controller.ViewData[ViewDataConstants.Module.Current] = module;
            controller.ViewData[ViewDataConstants.Module.Parent] = controller.GetParentModule();
        }

        public static string GetExecutingModule(this ModulesController controller)
        {
            return controller.RouteData.Values[RouteDataConstants.Module.Current] as string;
        }

        public static string GetName(this ModulesController controller)
        {
            return controller.GetType().Name.Replace("Controller", string.Empty);
        }

        public static string GetViewPath(this ModulesController controller)
        {
            return controller.RouteData.Values[RouteDataConstants.Controller.ViewPath] as string;
        }

        public static string GetViewPathDirectory(this ModulesController controller)
        {
            return controller.RouteData.Values[RouteDataConstants.Controller.ViewPathDirectory] as string;
        }

        public static void SetControllerAction(this ModulesController controller, string action)
        {
            controller.RouteData.Values[RouteDataConstants.Controller.Action] = action;
        }

        public static string GetControllerAction(this ModulesController controller)
        {
            return controller.RouteData.Values[RouteDataConstants.Controller.Action] as string;
        }


        public static string GetContentFullname(this ModulesController controller)
        {
            return controller.RouteData.Values[RouteDataConstants.Model.Fullname] as string;
        }

        public static string GetTargetType(this ModulesController controller)
        {
            return controller.RouteData.Values[RouteDataConstants.Model.TargetType] as string;
        }

        public static string GetModuleName(this ModulesController controller)
        {
            var moduleName = controller.GetName();
            return nameof(ModulesController).Replace("Controller", string.Empty)
                .Equals(moduleName, StringComparison.InvariantCultureIgnoreCase)
                ? controller.GetExecutingModule()
                : moduleName;
        }

        public static Type GetPassedModelType(this ModulesController controller)
        {
            var modelTypeQualifiedName = controller.GetTargetType();
            return string.IsNullOrWhiteSpace(modelTypeQualifiedName) ? null : Type.GetType(modelTypeQualifiedName);
        }

        public static Type GetPublishedContentType(this ModulesController controller)
        {
            return AppDomain.CurrentDomain.FindImplementations<PublishedContentModel>()
                .FirstOrDefault(type =>
                    type.Name.Equals(controller.GetModuleName(), StringComparison.InvariantCultureIgnoreCase));
        }

        public static Type GetPocoModelType(this ModulesController controller)
        {
            return AppDomain.CurrentDomain.FindImplementations<IModuleModel>()
                .FirstOrDefault(type =>
                    type.Name.Equals(controller.GetModuleName(), StringComparison.InvariantCultureIgnoreCase));
        }

        public static string GetParentModule(this ModulesController controller)
        {
            return controller.RouteData.Values[RouteDataConstants.Module.Parent] as string;
        }

    }
}