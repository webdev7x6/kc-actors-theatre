using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KCActorsTheatre.Blog
{
    public class Post
    {
        [Key]
        public int PostID { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public DateTime? PublishDate { get; set; }

        public DateTime? UnpublishDate { get; set; }

        public PublishStatus PublishStatus { get; set; }

        public Author Author { get; set; }
        public int? AuthorID { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        public string Summary { get; set; }
        
        public string Body { get; set; }

        public string ImageURL { get; set; }

        public bool IsViewable
        {
            get
            {
                var isPastPublishDate = (PublishDate != null && PublishDate < DateTime.UtcNow);
                var isPastUnpublishDate = (UnpublishDate != null && UnpublishDate < DateTime.UtcNow);

                return (isPastPublishDate && !isPastUnpublishDate && PublishStatus == PublishStatus.Published);
            }
        }

        private HashSet<Comment> _comments = new HashSet<Comment>();
        public ICollection<Comment> Comments { get { return _comments; } }
    }

    public enum PublishStatus
    {
        [Description("Draft")]
        Draft = 0,
        [Description("Published")]
        Published = 1
    }
}