using KCActorsTheatre.Tags;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KCActorsTheatre.Calendar
{
    public class CalendarEvent
    {
        public int CalendarEventID { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public DateTime? DateModified { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(150)]
        public string Title { get; set; }

        [MaxLength(100)]
        public string Location { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        public string LinkTarget { get; set; }
        public string LinkURL { get; set; }

        public int EventCategoryID { get; set; }
        public EventCategory EventCategory { get; set; }

        private HashSet<Tag> _tags = new HashSet<Tag>();
        public ICollection<Tag> Tags { get { return _tags; } }
    }
}