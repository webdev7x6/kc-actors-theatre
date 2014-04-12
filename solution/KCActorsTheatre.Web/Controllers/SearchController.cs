using Clickfarm.Cms.Core;
using KCActorsTheatre.Web.ViewModels;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace KCActorsTheatre.Web.Controllers
{
    public class SearchController : KCActorsTheatreController
    {
        public SearchController(ICmsContext context, HttpContextBase httpContext) : base(context, httpContext) { }

        [HttpGet]
        public ActionResult Index(string q)
        {
            var model = new SearchViewModel();
            InitializeViewModel(model);
            model.SearchQuery = q;

            string apiKey = "AIzaSyCMGfdDaSfjqv5zYoS0mTJnOT3e9MURWkU";
            string cx = "004210632643459701224:qmfpfiwzy-y";

            Google.Apis.Customsearch.v1.CustomsearchService svc = new Google.Apis.Customsearch.v1.CustomsearchService(new Google.Apis.Services.BaseClientService.Initializer() { ApiKey = apiKey });

            Google.Apis.Customsearch.v1.CseResource.ListRequest listRequest = svc.Cse.List(q);
            listRequest.Cx = cx;
            Google.Apis.Customsearch.v1.Data.Search search = listRequest.Execute();

            var searchResults = new List<Google.Apis.Customsearch.v1.Data.Result>();

            foreach (Google.Apis.Customsearch.v1.Data.Result result in search.Items)
                searchResults.Add(result);

            model.SearchResults = searchResults;

            return View(model);
        }
    }
}