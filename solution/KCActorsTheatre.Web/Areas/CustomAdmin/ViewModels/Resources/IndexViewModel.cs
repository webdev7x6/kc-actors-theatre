using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KCActorsTheatre.Resources;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Resources
{
    public class IndexViewModel : AdminViewModel
    {
        public Resource Resource = new Resource();
        public ResourceType ResourceType { get; set; }
        public string DescriptionSingular { get; set; }
        public string DescriptionPlural { get; set; }
        public Type TypeofResource { get; set; }
    }
}