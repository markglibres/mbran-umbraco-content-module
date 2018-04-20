using Umbraco.Core.Models;

namespace MBran.ContentModule.Models
{
    public interface IModuleModel
    {
        IPublishedContent Content { get; set; }
    }
}
