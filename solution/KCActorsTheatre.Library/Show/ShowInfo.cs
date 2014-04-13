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
        [Key]
        public int ShowId { get; set; }
        public DateTime DateCreated { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime? ShowDate { get; set; }
        public string Summary { get; set; }
        public string Author { get; set; }
        public bool IsPublished { get; set; }
        //public DateTime? EndDate { get; set; }
    }
}