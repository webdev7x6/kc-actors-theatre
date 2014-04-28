using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Clickfarm.Cms.Core;
using KCActorsTheatre.Web.ViewModels;

namespace KCActorsTheatre.Web.Controllers
{
    public class SeasonController : KCActorsTheatreController
    {
        public SeasonController(ICmsContext context, HttpContextBase httpContext)
            : base(context, httpContext) 
        {
        }

        public ActionResult Item(int id)
        {
            var model = new SeasonViewModel();
            InitializeViewModel(model);

            try
            {
                var repoReponse = repository.Seasons.GetSingle(id);
                if (repoReponse.Succeeded && repoReponse.Entity != null)
                    model.Season = repoReponse.Entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View("Item", model);
        }
    }
}