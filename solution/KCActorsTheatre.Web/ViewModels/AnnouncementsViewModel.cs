using KCActorsTheatre.Announcements;
using System.Collections.Generic;

namespace KCActorsTheatre.Web.ViewModels
{
    public class AnnouncementsViewModel : KCActorsTheatreViewModel
    {
        public IEnumerable<Announcement> Announcements { get; set; }
        public AnnouncementType AnnouncementType { get; set; }
    }
}