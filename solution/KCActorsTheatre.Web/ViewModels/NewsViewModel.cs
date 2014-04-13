using KCActorsTheatre.News;
using System.Collections.Generic;

namespace KCActorsTheatre.Web.ViewModels
{
    public class NewsViewModel : KCActorsTheatreViewModel
    {
        public Article Article { get; set; }

        public bool HasArticle
        {
            get { return Article != null; }
        }
    }
}