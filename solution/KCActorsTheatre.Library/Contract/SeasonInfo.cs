using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KCActorsTheatre.Contract
{
    public class SeasonInfo
    {
        [Key]
        public int SeasonID { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        public string MainImageUrl { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime DateCreated { get; set; }

        private HashSet<ShowInfo> _shows = new HashSet<ShowInfo>();
        public ICollection<ShowInfo> Shows { get { return _shows; } } 
    }
}