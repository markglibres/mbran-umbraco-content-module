using Umbraco.Core.Models;

namespace MBran.ContentModule.Models
{
    public class ModuleModel : IModuleModel
    {
        public IPublishedContent Content { get; set; }
    }
}