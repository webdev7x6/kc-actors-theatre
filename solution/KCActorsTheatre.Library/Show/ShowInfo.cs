using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCActorsTheatre.Show
{
    public class ShowInfo
    {
        public int ShowId { get; set; }
        public DateTime DateCreated { get; set; }
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        public string Summary { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
