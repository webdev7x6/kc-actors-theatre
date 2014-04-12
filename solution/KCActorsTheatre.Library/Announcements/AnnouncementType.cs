using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCActorsTheatre.Announcements
{
    public enum AnnouncementType
    {
        [Description("Announcement")]
        Announcement,
        [Description("Official")]
        Official,
    }
}