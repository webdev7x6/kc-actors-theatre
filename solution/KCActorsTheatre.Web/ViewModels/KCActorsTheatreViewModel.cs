using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Announcements;
using KCActorsTheatre.Cms.ContentTypes;
using KCActorsTheatre.Resources;
using KCActorsTheatre.Staff;
using System.Collections.Generic;

namespace KCActorsTheatre.Web.ViewModels
{
    public class KCActorsTheatreViewModel : BaseViewModel
    {
        public string HeroImageURL { get; set; }
        public string HeroImageTitle { get; set; }
        public string HeroImageCaption { get; set; }

        public List<string> ResourceWidgetTags { get; set; }
        public IEnumerable<Resource> ResourceWidgetResources { get; set; }

        public IEnumerable<ContentWidgetContent> ContentWidgets { get; set; }

        public List<string> AnnouncementWidgetTags { get; set; }
        public IEnumerable<Announcement> AnnouncementWidgetAnnouncements { get; set; }

        public IEnumerable<StaffMember> ConnectWidgetContacts { get; set; }
    }
}