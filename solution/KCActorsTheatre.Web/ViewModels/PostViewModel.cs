using KCActorsTheatre.Blogs;
using System.Collections.Generic;

namespace KCActorsTheatre.Web.ViewModels
{
    public class PostViewModel : KCActorsTheatreViewModel
    {
        public Post Post { get; set; }
        public Post PreviousPost { get; set; }
        public Post NextPost { get; set; }
        public string DisqusAccount { get; set; }
    }
}