using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using KCActorsTheatre.News;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.News
{
    public class NewsViewModel : AdminViewModel
    {
        public Article Article { get; set; }
    }
}