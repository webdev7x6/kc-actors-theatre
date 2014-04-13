using System;
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
        public string Summary { get; set; }
        public string Reviews { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsPublished { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
