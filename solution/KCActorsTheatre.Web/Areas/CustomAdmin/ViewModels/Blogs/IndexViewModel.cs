using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using KCActorsTheatre.Blogs;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Blogs
{
    public class IndexViewModel : AdminViewModel
    {
        public Post Post = new Post();
        public BlogType BlogType { get; set; }
        public string DescriptionSingular { get; set; }
        public string DescriptionPlural { get; set; }
    }
}