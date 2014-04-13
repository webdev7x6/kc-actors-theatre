using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Contract;
using KCActorsTheatre.Data;
using System.Collections.Generic;

namespace KCActorsTheatre.Web.ViewModels
{
    public class ShowViewModel : KCActorsTheatreViewModel
    {
        public IEnumerable<ShowInfo> Shows { get; set; }
        public ShowInfo Show { get; set; }
    }
}