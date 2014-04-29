using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Contract;
using KCActorsTheatre.Data;
using System.Collections.Generic;

namespace KCActorsTheatre.Web.ViewModels
{
    public class PersonViewModel : KCActorsTheatreViewModel
    {
        public Person Person { get; set; }
    }
}