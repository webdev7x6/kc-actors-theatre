using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KCActorsTheatre.Blog
{
    public class Author
    {
        [Key]
        public int AuthorID { get; set; }

        public DateTime DateCreated { get; set; }
        
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageURL { get; set; }

        private HashSet<Post> _posts = new HashSet<Post>();
        public ICollection<Post> Posts { get { return _posts; } }
    }
}