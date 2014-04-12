using Clickfarm.Cms.Core;
using KCActorsTheatre.Staff;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
using System.Web;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.StaffMembers
{
    public class EditViewModel
    {
        public StaffMember StaffMember { get; set; }

        public bool HasStaffMember
        {
            get { return StaffMember != null; }
        }

        public IUtcDateConverter DateConverter { get; set; }

        public ImageContentProperties ContentProperties_ThumbnailImage { get; set; }

        public FileContentProperties ContentProperties_ThumbnailImageFile { get; set; }

        public FileContentProperties ContentProperties_ResourceFile { get; set; }
    }
}