using Clickfarm.Cms.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KCActorsTheatre.Library.AppTypes;
using KCActorsTheatre.Contract;

namespace KCActorsTheatre.Web.ViewModels
{
    public class KCActorsTheatreViewModel : BaseViewModel
    {
        public IEnumerable<SeasonInfo> Seasons { get; set; }
        public IEnumerable<ShowInfo> CurrentShows { get; set; }
    }
}