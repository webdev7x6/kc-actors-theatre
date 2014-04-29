using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using KCActorsTheatre.Contract;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.PersonModel
{
    public class PersonViewModel : AdminViewModel
    {
        public Person Person { get; set; }
        public RoleDefinition RoleDefinition { get; set; }
        public IEnumerable<SelectListItem> ShowList;

    }
}