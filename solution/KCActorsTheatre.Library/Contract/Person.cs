using System;
using System.ComponentModel.DataAnnotations;

namespace KCActorsTheatre.Contract
{
    public class Person
    {
        public int PersonID { get; set; }

        public string Title { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        public string ImageUrl { get; set; }
        public string BioDetail { get; set; }
        public string BioSummary { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
