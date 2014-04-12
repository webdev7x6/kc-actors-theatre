using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KCActorsTheatre.Tags;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Tags
{
    public class IndexViewModel : AdminViewModel
    {
        public Tag Tag = new Tag();
        public TagType TagType { get; set; }
        public string DescriptionSingular { get; set; }
        public string DescriptionPlural { get; set; }
    }
}