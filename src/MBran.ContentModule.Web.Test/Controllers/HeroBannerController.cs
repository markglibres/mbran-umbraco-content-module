using System.Web.Mvc;
using MBran.ContentModule.Controller;
using Umbraco.Web.PublishedContentModels;

namespace MBran.ContentModule.Web.Test.Controllers
{
    public class HeroBannerController : ModulesController
    {
        public override PartialViewResult Index()
        {
            var model = GetStronglyTypedModel();
            return ModuleView(model);
        }
    }
}