using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KCActorsTheatre.Organizations;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Organizations
{
    public class IndexViewModel : AdminViewModel
    {
        public Organization Organization = new Organization();
        public OrganizationType OrganizationType { get; set; }
        public string DescriptionSingular { get; set; }
        public string DescriptionPlural { get; set; }
    }
}