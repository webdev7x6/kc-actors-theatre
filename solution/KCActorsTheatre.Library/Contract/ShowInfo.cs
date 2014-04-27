using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KCActorsTheatre.Contract
{
    public class ShowInfo
    {
        [Key]
        public int ShowID { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        public string Body { get; set; }
        public string MainImageUrl { get; set; }
        public string Summary { get; set; }
        public string SummaryImageUrl { get; set; }
        public string Reviews { get; set; }
        public string Venue { get; set; }
        public string DatesAndTimes { get; set; }
        public string Quote { get; set; }
        public string Education { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsPublished { get; set; }
        public DateTime DateCreated { get; set; }

        public int? SeasonID { get; set; }
        public SeasonInfo Season { get; set; }

        private HashSet<Person> _people = new HashSet<Person>();
        public ICollection<Person> People { get { return _people; } }

        private HashSet<ShowImage> _images = new HashSet<ShowImage>();
        public ICollection<ShowImage> Images { get { return _images; } }

        private HashSet<ShowVideo> _videos = new HashSet<ShowVideo>();
        public ICollection<ShowVideo> Videos{ get { return _videos; } }
    }
}