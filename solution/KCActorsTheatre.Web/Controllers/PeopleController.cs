using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Clickfarm.Cms.Core;
using KCActorsTheatre.Web.ViewModels;

namespace KCActorsTheatre.Web.Controllers
{
    public class PeopleController : KCActorsTheatreController
    {
        public PeopleController(ICmsContext context, HttpContextBase httpContext)
            : base(context, httpContext) 
        {
        }

        public ActionResult Index()
        {
            var model = new PeopleViewModel();
            InitializeViewModel(model);
            model.People = repository.People.GetForWebsite(9, 0).Entity;
            return View(model);
        }

        public ActionResult Item(int id)
        {
            var model = new PersonViewModel();
            InitializeViewModel(model);

            try
            {
                var repoReponse = repository.People.GetSingle(id);
                if (repoReponse.Succeeded && repoReponse.Entity != null)
                    model.Person = repoReponse.Entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View("Item", model);
        }
    }
}