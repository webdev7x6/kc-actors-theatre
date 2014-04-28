﻿using System;
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

        public KCActorsTheatreController(ICmsContext context, HttpContextBase httpContext)
        {
            this.context = this.context;
            this.repository = context.Repository as IKCActorsTheatreRepository;
            this.session = httpContext.Session;

            // set session objects
            //if (RecentPosts == null)
            //    RecentPosts = repository.Posts.GetPostedAndPublished(3, 0).Entity.ToList();
        }

        protected void InitializeViewModel(KCActorsTheatreViewModel model)
        {
            // initialize common viewmodel objects
            model.RequestContent = this.CmsRequestContent;
            model.Seasons = GetSeasons();
            model.CurrentShows = GetCurrentShows();
        }

        private IEnumerable<SeasonInfo> GetSeasons()
        {
            return repository.Seasons.All();
        }

        private IEnumerable<ShowInfo> GetCurrentShows()
        {
            return repository.Shows.All();
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