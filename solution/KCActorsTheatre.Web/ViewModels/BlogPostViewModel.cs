using KCActorsTheatre.Blog;
using System.Collections.Generic;

namespace KCActorsTheatre.Web.ViewModels
{
    public class BlogPostViewModel : KCActorsTheatreViewModel
    {
        public Post Post { get; set; }
        public Comment Comment = new Comment();

        public bool HasPost
        {
            get { return Post != null; }
        }

        public int? NextPostID { get; set; }
        public int? PreviousPostID { get; set; }
    }
}