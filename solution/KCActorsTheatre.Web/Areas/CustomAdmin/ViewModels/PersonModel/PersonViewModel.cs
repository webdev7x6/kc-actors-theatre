using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using KCActorsTheatre.Contract;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.PersonModel
{
    public class PersonViewModel : AdminViewModel
    {
        public Person Person { get; set; }
    }
}