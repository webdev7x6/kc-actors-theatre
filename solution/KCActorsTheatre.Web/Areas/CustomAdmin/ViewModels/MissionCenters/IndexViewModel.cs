using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KCActorsTheatre.Locations;
using System.Web.Mvc;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.MissionCenters
{
    public class IndexViewModel : AdminViewModel
    {
        public MissionCenter MissionCenter = new MissionCenter();
        public IEnumerable<SelectListItem> MissionFieldList;
    }
}