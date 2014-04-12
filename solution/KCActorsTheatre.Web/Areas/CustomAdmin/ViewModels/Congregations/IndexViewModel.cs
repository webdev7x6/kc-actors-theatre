using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KCActorsTheatre.Locations;
using System.Web.Mvc;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Congregations
{
    public class IndexViewModel : AdminViewModel
    {
        public Congregation Congregation = new Congregation();
        public CongregationContact CongregationContact = new CongregationContact();
        public IEnumerable<SelectListItem> MissionCenterList;
    }
}