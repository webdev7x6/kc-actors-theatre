using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Calendar;
using KCActorsTheatre.Data;
using KCActorsTheatre.Library.AppTypes;
using System.Collections.Generic;

namespace KCActorsTheatre.Web.ViewModels
{
    public class HomeViewModel : KCActorsTheatreViewModel
    {
        public IEnumerable<Event> Events { get; set; }
    }
}