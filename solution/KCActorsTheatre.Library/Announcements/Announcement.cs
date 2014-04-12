using KCActorsTheatre.Resources;
using KCActorsTheatre.Tags;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace KCActorsTheatre.Announcements
{
    public class Announcement
    {
        public int AnnouncementID { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        public DateTime? DatePublished { get; set; }
        public DateTime? DateExpired { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }

        public AnnouncementType AnnouncementType { get; set; }

        private HashSet<Tag> _tags = new HashSet<Tag>();
        public ICollection<Tag> Tags { get { return _tags; } }

        [NotMapped]
        public bool IsPublished 
        { 
            get 
            {
                return DatePublished.HasValue
                    && DatePublished.Value <= DateTime.UtcNow
                    && (
                        (DateExpired.HasValue
                        && DateExpired.Value >= DateTime.UtcNow)
                        ||
                        !DateExpired.HasValue
                    )
                ;
            }
        }
    }
}