using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using MBran.ContentModule.Models;
using MBran.Core.Extensions;
using Our.Umbraco.Ditto;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;

namespace MBran.ContentModule.Controller
{
    public class DefaultModuleController : ModuleController, IRenderModule
    {
        public object GetViewModel()
        {
            var modelType = GetModelType();
            return modelType != null ? GetContent().As(modelType) : GetContent();
        }

        public PartialViewResult Render()
        {
            SetControllerAction(GetModuleName());
            return PartialView(GetView(), GetViewModel());
        }
    }
}