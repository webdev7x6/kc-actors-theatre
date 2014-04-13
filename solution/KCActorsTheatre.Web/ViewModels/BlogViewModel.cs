using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Blog;
using KCActorsTheatre.Data;
using System.Collections.Generic;

namespace KCActorsTheatre.Web.ViewModels
{
    public class BlogViewModel : KCActorsTheatreViewModel
    {
        public IEnumerable<Post> Posts { get; set; }
        public object JsonPosts { get; set; }
    }
}