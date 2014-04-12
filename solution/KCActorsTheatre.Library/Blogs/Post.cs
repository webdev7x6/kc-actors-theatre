using KCActorsTheatre.Tags;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KCActorsTheatre.Blogs
{
    public class Post
    {
        public int PostID { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        [StringLength(100)]
        public string Author { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        [StringLength(200)]
        public string Teaser { get; set; }

        public string Body { get; set; }

        public DateTime? DateToPost { get; set; }

        public DateTime? DateExpires { get; set; }

        public BlogType BlogType { get; set; }

        public ICollection<PostCategory> Categories { get; private set; }

        public ICollection<Tag> Tags { get; private set; }

        public PostStatus Status { get; set; }

        [NotMapped]
        public string PreviousPostURL { get; set; }
        [NotMapped]
        public string NextPostURL { get; set; }
    }

    public class PostsForPage
    {
        public Post Post { get; set; }
        public Post PreviousPost { get; set; }
        public Post NextPost { get; set; }
    }
}