using MBran.ContentModule.Constants;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace MBran.ContentModule.Controller
{
    public abstract class BaseContentModuleController : SurfaceController
    {
        public void SetExecutingModule(string module)
        {
            ViewData[RouteDataConstants.Module.Current] = module;
        }

        public string GetExecutingModule()
        {
            return ViewData[RouteDataConstants.Module.Current] as string;
        }

        public string GetName()
        {
            return GetType().Name.Replace("Controller", string.Empty);
        }

        public string GetViewPath()
        {
            return ViewData[RouteDataConstants.Controller.ViewPath] as string;
        }

        public void SetControllerAction(string action)
        {
            RouteData.Values[RouteDataConstants.Controller.Action] = action;
        }

        public string GetControllerAction()
        {
            return RouteData.Values[RouteDataConstants.Controller.Action] as string;
        }

        public IPublishedContent GetContent()
        {
            return ControllerContext.RouteData.Values[RouteDataConstants.Model.Request] as IPublishedContent
                   ?? UmbracoContext.Current.PublishedContentRequest.PublishedContent;
        }

        public string GetContentFullname()
        {
            return RouteData.Values[RouteDataConstants.Model.Fullname] as string;
        }

    }
}