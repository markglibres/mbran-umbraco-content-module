using MBran.ContentModule.Constants;
using MBran.ContentModule.Controller;

namespace MBran.ContentModule.Extensions
{
    public static class ModulesControllerExtensions
    {
        public static void SetExecutingModule(this ModulesController controller, string module)
        {
            controller.ViewData[ViewDataConstants.Module.Current] = module;
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
    }
}