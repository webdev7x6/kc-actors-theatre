using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Blog;
using KCActorsTheatre.Data;
using System.Collections.Generic;

namespace KCActorsTheatre.Web.ViewModels
{
    public class NewsViewModel : KCActorsTheatreViewModel
    {
        public IEnumerable<Post> NewsArticles { get; set; }
    }
}