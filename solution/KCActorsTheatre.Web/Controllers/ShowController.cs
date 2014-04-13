using Clickfarm.AppFramework.Responses;
using Clickfarm.AppFramework.Web;
using Clickfarm.Cms.Core;
using Clickfarm.AppFramework.Extensions;
using KCActorsTheatre.Contract;
using KCActorsTheatre.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCActorsTheatre.Web.Controllers
{
    public class ShowController : KCActorsTheatreController
    {
        public ShowController(ICmsContext context, HttpContextBase httpContext)
            : base(context, httpContext) 
        {
        }

        public ActionResult Index()
        {
            var model = new ShowViewModel();
            InitializeViewModel(model);

            model.Shows = repository.Shows.GetForWebsite(9, 0).Entity;

            return View(model);
        }

        public ActionResult Item(int id)
        {
            var model = new ShowViewModel();
            InitializeViewModel(model);

            try
            {
                var repoReponse = repository.Shows.GetSingle(id);
                if (repoReponse.Succeeded && repoReponse.Entity != null)
                    model.Show = repoReponse.Entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View("Item", model);
        }

        private string Truncate(string value, int maxChars)
        {
            var returnValue = string.Empty;
            if (!string.IsNullOrWhiteSpace(value))
                returnValue = value.Length <= maxChars ? value : value.Substring(0, maxChars) + " ..";
            return returnValue;
        }
    }
}