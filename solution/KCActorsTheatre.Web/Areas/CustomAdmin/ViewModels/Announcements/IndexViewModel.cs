using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KCActorsTheatre.Announcements;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Announcements
{
    public class IndexViewModel : AdminViewModel
    {
        public Announcement Announcement = new Announcement();
        public AnnouncementType AnnouncementType { get; set; }
        public string DescriptionSingular { get; set; }
        public string DescriptionPlural { get; set; }
    }
}