using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Mvc;

namespace MBran.ContentModule.Controller
{
    public class ContentModuleController : SurfaceController, IContentModule
    {
        public string GetModuleName()
        {
            throw new NotImplementedException();
        }

        public string GetViewName()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetViewPathLocations()
        {
            throw new NotImplementedException();
        }
    }
}
