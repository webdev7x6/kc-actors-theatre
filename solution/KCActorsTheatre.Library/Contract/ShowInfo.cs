using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KCActorsTheatre.Contract
{
    public class ShowInfo
    {
        [Key]
        public int ShowId { get; set; }

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

        private HashSet<Person> _people = new HashSet<Person>();
        public ICollection<Person> People { get { return _people; } }

        private HashSet<ShowMedia> _showMedia = new HashSet<ShowMedia>();
        public ICollection<ShowMedia> ShowMedia { get { return _showMedia; } }
    }

    public class ShowMedia
    {
        public int ShowMediaID { get; set; }
        public MediaType MediaType { get; set; }
    }

    public enum MediaType
    {
        [Description("Image")]
        Image,
        [Description("Video")]
        Video
    }
}