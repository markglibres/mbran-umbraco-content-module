using System.Web.Mvc;

namespace MBran.ContentModule.Controller
{
    public interface IRenderModule
    {
        PartialViewResult Render();
        object GetViewModel();
    }
}