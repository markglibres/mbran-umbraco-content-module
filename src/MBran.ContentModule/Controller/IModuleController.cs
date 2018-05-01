using System.Collections.Generic;
using System.Web.Mvc;
using Umbraco.Core.Models;
using System;

namespace MBran.ContentModule.Controller
{
    public interface IModuleController
    {
        string GetView(string view = null);
        string GetModuleName();
        IEnumerable<string> GetViewPathLocations();
        IPublishedContent PublishedContent { get; }
        Type GetModelType();
        PartialViewResult Index();
    }
}