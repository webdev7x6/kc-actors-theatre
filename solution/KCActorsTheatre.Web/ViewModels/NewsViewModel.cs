using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.News;
using KCActorsTheatre.Data;
using System.Collections.Generic;

namespace KCActorsTheatre.Web.ViewModels
{
    public class NewsViewModel : KCActorsTheatreViewModel
    {
        public IEnumerable<Article> NewsArticles { get; set; }
        public Article Article { get; set; }
    }
}