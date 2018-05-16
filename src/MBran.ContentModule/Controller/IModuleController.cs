using System.Web.Mvc;

namespace MBran.ContentModule.Controller
{
    public interface IModuleController
    {
        PartialViewResult Index();
        PartialViewResult ModuleView(string viewName, object model);
        PartialViewResult ModuleView(object model);
    }
}