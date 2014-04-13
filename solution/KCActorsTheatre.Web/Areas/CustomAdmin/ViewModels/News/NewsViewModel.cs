using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using KCActorsTheatre.Contract;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.News
{
    public class NewsViewModel : AdminViewModel
    {
        public Article Article { get; set; }
    }
}