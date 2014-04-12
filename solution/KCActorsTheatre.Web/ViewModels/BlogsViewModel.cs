using KCActorsTheatre.Blogs;
using System.Collections.Generic;

namespace KCActorsTheatre.Web.ViewModels
{
    public class BlogsViewModel : KCActorsTheatreViewModel
    {
        public IEnumerable<PostCategory> Categories { get; set; }
        public Dictionary<int, Dictionary<int, int>> ArchiveData { get; set; }
        public BlogType BlogType { get; set; }
        public string DisqusAccount { get; set; }
    }
}