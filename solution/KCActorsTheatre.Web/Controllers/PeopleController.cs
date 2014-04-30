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

        public ActionResult Index(string title)
        {
            var model = new PeopleViewModel();
            InitializeViewModel(model);
            model.People = string.IsNullOrWhiteSpace(title) 
                ? repository.People.GetAll().Entity 
                : repository.People.GetByTitle(title).Entity;
            return View(model);
        }

        public ActionResult Person(int id)
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
            return View(model);
        }

        public ActionResult Founders()
        {
            var model = new PeopleViewModel();
            InitializeViewModel(model);
            model.People = repository.People.GetByTitle("Founder").Entity;
            return View(model);
        }

        public ActionResult Board()
        {
            var model = new PeopleViewModel();
            InitializeViewModel(model);
            model.People = repository.People.GetByTitle("Board").Entity;
            return View(model);
        }

        public ActionResult Artists()
        {
            var model = new PeopleViewModel();
            InitializeViewModel(model);
            model.People = repository.People.GetByTitle("Artist").Entity;
            return View(model);
        }
    }
}