using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Contract;
using KCActorsTheatre.Data;
using KCActorsTheatre.Library.AppTypes;
using System.Collections.Generic;

namespace KCActorsTheatre.Web.ViewModels
{
    public class HomeViewModel : KCActorsTheatreViewModel
    {
        public IEnumerable<ImageContent> RotatorImages { get; set; }
        public SeasonInfo Season { get; set; }
    }
}