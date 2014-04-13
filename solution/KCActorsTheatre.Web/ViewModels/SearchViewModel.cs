using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Data;
using KCActorsTheatre.Library.AppTypes;
using System.Collections.Generic;

namespace KCActorsTheatre.Web.ViewModels
{
    public class SearchViewModel : KCActorsTheatreViewModel
    {
        public IEnumerable<Google.Apis.Customsearch.v1.Data.Result> SearchResults { get; set; }
        public string SearchQuery { get; set; }
    }
}