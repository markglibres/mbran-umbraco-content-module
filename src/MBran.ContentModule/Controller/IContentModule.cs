using System.Collections.Generic;

namespace MBran.ContentModule.Controller
{
    public interface IContentModule
    {
        string GetViewName();
        string GetModuleName();
        IEnumerable<string> GetViewPathLocations();
    }
}