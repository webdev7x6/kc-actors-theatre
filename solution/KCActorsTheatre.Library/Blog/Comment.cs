using System;
using System.ComponentModel.DataAnnotations;

namespace KCActorsTheatre.Blog
{
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public DateTime? DateApproved { get; set; }

        public bool IsApproved { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Comment is required.")]
        public string Text { get; set; }

        public Post Post { get; set; }
        [Required]
        public int PostID { get; set; }
    }
}