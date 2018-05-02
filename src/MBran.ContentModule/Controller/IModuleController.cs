using System.Web.Mvc;

namespace MBran.ContentModule.Controller
{
    public interface IModuleController
    {
        PartialViewResult Index();
    }
}