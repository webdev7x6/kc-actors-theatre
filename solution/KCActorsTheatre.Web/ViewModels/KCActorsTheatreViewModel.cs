using Clickfarm.Cms.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KCActorsTheatre.Blog;
using KCActorsTheatre.Library.AppTypes;

namespace KCActorsTheatre.Web.ViewModels
{
    public class KCActorsTheatreViewModel : BaseViewModel
    {
        public IEnumerable<Post> RecentPosts { get; set; }
    }
}