using KCActorsTheatre.Resources;
using KCActorsTheatre.Tags;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCActorsTheatre.MissionStories
{
    public class MissionStory
    {
        public int MissionStoryID { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DatePublished { get; set; }

        public int? DisplayOrder { get; set; }

        public string Title { get; set; }
        public string Teaser { get; set; }
        public string Body { get; set; }

        public string ThumbnailImageURL { get; set; }
        public string ImageURL { get; set; }

        public int? ResourceID { get; set; }
        public Resource Resource { get; set; }

        private HashSet<Tag> _tags = new HashSet<Tag>();
        public ICollection<Tag> Tags { get { return _tags; } }

        [NotMapped]
        public bool IsPublished
        {
            get
            {
                return DatePublished.HasValue
                    && DatePublished.Value <= DateTime.UtcNow
                ;
            }
        }
    }
}