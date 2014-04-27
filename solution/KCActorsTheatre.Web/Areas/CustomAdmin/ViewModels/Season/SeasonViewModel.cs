using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using KCActorsTheatre.Contract;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Season
{
    public class SeasonViewModel : AdminViewModel
    {
        public SeasonInfo Season { get; set; }
    }
}