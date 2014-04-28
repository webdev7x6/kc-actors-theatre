using System;
using System.Web.Mvc;
using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Logging;
using Clickfarm.AppFramework.Web;
using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Data;
using KCActorsTheatre.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KCActorsTheatre.Contract;

namespace KCActorsTheatre.Web.Controllers
{
    public class KCActorsTheatreController : CmsController
    {
        protected ICmsContext context { get; private set; }
        protected IKCActorsTheatreRepository repository { get; private set; }
        private HttpSessionStateBase session = null;
        private HttpApplicationStateBase application = null;

        public KCActorsTheatreController(ICmsContext context, HttpContextBase httpContext)
        {
            this.context = this.context;
            this.repository = context.Repository as IKCActorsTheatreRepository;
            this.session = httpContext.Session;
            this.application = httpContext.Application;

            if (CurrentSeason == null)
                CurrentSeason = repository.Seasons.GetCurrent().Entity;

            if (PastSeasons == null)
                PastSeasons = repository.Seasons.GetPastSeasons().Entity;
        }

        protected void InitializeViewModel(KCActorsTheatreViewModel model)
        {
            // initialize common viewmodel objects
            model.RequestContent = this.CmsRequestContent;

            // application objects for the nav
            model.PastSeasons = PastSeasons;
            model.CurrentSeason = CurrentSeason;
            model.CurrentShows = CurrentSeason.Shows;
        }

        public SeasonInfo CurrentSeason
        {
            get
            {
                SeasonInfo season = (SeasonInfo)application["CurrentSeason"];
                return season;
            }
            private set
            {
                application["CurrentSeason"] = value;
            }
        }

        public IEnumerable<SeasonInfo> PastSeasons
        {
            get
            {
                IEnumerable<SeasonInfo> seasons = (IEnumerable<SeasonInfo>)application["PastSeasons"];
                return seasons;
            }
            private set
            {
                application["PastSeasons"] = value;
            }
        }

        private JsonResult CatchError(Func<JsonResult> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                DependencyResolver.Current.GetService<ILogger>().LogWeb(HttpContext, LogLevel.Error, "A JSON response exception occurred on KCActorsTheatre.com.", ex.GetInnermostException());
                return new JsonResult
                {
                    Data = new ErrorViewModel(CmsRequestContent, ex),
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }
    }
}