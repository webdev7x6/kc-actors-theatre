using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KCActorsTheatre.Staff;
using System.Web.Mvc;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.StaffMembers
{
    public class IndexViewModel : AdminViewModel
    {
        public StaffMember StaffMember = new StaffMember();
        public StaffType StaffType { get; set; }

        public RoleDefinition RoleDefinition = new RoleDefinition();
        public IEnumerable<SelectListItem> MissionCenterList;
        public IEnumerable<SelectListItem> MissionFieldList;
        public IEnumerable<string> RoleTitles;
        public StaffMember EditStaffMember = null;

        public string DescriptionSingular { get; set; }
        public string DescriptionPlural { get; set; }

        public Type TypeofStaffMember { get; set; }
    }
}