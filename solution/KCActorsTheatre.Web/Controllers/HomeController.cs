using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using Clickfarm.AppFramework.Web;
using Clickfarm.Cms.Core;
using KCActorsTheatre.Web.ViewModels;
using System;
using System.Web;
using System.Web.Mvc;

namespace KCActorsTheatre.Web.Controllers
{
    public class HomeController : KCActorsTheatreController
    {
        public HomeController(ICmsContext context, HttpContextBase httpContext) : base(context, httpContext) { }

        public ActionResult Index()
        {
            var model = new HomeViewModel();
            InitializeViewModel(model);
            return View(model);
        }

        public ActionResult Inner()
        {
            var model = new InnerViewModel();
            InitializeViewModel(model);
            return View(model);
        }

        public ActionResult SiteMap()
        {
            var model = new HomeViewModel();
            InitializeViewModel(model);
            return View(model);
        }

        public ActionResult CommunityDashboard()
        {
            var model = new InnerViewModel();
            InitializeViewModel(model);
            return View(model);
        }

        //[AjaxOnly]
        //public JsonResult NewSignUp(NewsletterSignUp newsletterSignUp)
        //{
        //    JsonResponse jsonResponse = new JsonResponse();
        //    newsletterSignUp.DateCreated = DateTime.UtcNow;

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var repoReponse = repository.NewsletterSignUps.New(newsletterSignUp);

        //            if (repoReponse.Succeeded)
        //                jsonResponse.Succeeded = true;
        //            else
        //                jsonResponse.Fail(repoReponse.Message);
        //        }
        //        catch (Exception ex)
        //        {
        //            jsonResponse.Succeeded = false;
        //            jsonResponse.Message = ex.GetInnermostException().Message;
        //        }

        //    }
        //    else
        //    {
        //        jsonResponse.Fail("Validation failed.");
        //    }
        //    return Json(jsonResponse);
        //}
    }
}