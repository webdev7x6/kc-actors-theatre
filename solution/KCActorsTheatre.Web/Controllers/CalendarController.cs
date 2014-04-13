using Clickfarm.AppFramework.Responses;
using Clickfarm.AppFramework.Web;
using Clickfarm.Cms.Core;
using Clickfarm.AppFramework.Extensions;
using KCActorsTheatre.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCActorsTheatre.Calendar;

namespace KCActorsTheatre.Web.Controllers
{
    public class CalendarController : KCActorsTheatreController
    {
        public CalendarController(ICmsContext context, HttpContextBase httpContext)
            : base(context, httpContext) 
        {
        }

        public ActionResult Index()
        {
            var model = new CalendarViewModel();
            InitializeViewModel(model);

            model.Events = repository.Events.GetForWebsite(9, 0).Entity;
            model.JsonEvents = ConvertEvents(model.Events);

            return View(model);
        }

        public ActionResult Event(int id)
        {
            var model = new CalendarEventViewModel();
            InitializeViewModel(model);

            try
            {
                var repoReponse = repository.Events.GetSingle(id);
                if (repoReponse.Succeeded && repoReponse.Entity != null)
                    model.Event = repoReponse.Entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View(model);
        }

        [AjaxOnly]
        public JsonResult GetEvents(int howMany, int skip)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                var repoReponse = repository.Events
                    .GetForWebsite(howMany, skip)
                ;

                if (repoReponse.Succeeded && repoReponse.Entity != null)
                {
                    jsonResponse.Properties.Add("Items", ConvertEvents(repoReponse.Entity));
                    jsonResponse.Succeeded = true;
                }
            }
            catch (Exception ex)
            {
                jsonResponse.Succeeded = false;
                jsonResponse.Message = ex.GetInnermostException().Message;
            }

            return Json(jsonResponse);
        }

        [AjaxOnly]
        public JsonResult Search(string searchTerm)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                var repoReponse = repository.Events.FindForWebsite(searchTerm);

                if (repoReponse.Succeeded && repoReponse.Entity != null)
                {
                    jsonResponse.Properties.Add("Items", ConvertEvents(repoReponse.Entity));
                    jsonResponse.Succeeded = true;
                }
            }
            catch (Exception ex)
            {
                jsonResponse.Succeeded = false;
                jsonResponse.Message = ex.GetInnermostException().Message;
            }

            return Json(jsonResponse);
        }

        private IEnumerable<object> ConvertEvents(IEnumerable<Event> events)
        {
            return events.Select(a =>
            {
                return ConvertEvent(a);
            });
        }

        private object ConvertEvent(Event @event)
        {
            var startDate = @event.StartDate.Value;
            return new
            {
                ID = @event.EventID,
                StartDate = startDate.ToString("dd"),
                StartMonth = startDate.ToString("MMM"),
                Title = Truncate(@event.Title, 18),
                Summary = Truncate(@event.Summary, 70),
            };
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