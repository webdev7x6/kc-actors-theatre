using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using Clickfarm.Cms.Admin.Mvc.Areas.Admin;
using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Calendar;
using KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Calendar;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.Controllers
{
    [RequiresCmsUserAuthorization(Constants.CmsRole_CalendarManager)]
    public class CalendarController : KCActorsTheatreAdminControllerBase
    {
        public CalendarController(ICmsContext context) : base(context) { }

        public ViewResult Events()
        {
            var model = new EventsViewModel();
            model.Init(CmsContext);
            return View(model);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult CreateEventAjax(Event @event)
        {
            JsonResponse jsonResponse = new JsonResponse();

            @event.DateCreated = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                try
                {
                    var repoResponse = Repository.Events.New(@event);

                    if (repoResponse.Succeeded && repoResponse.Entity != null)
                    {
                        jsonResponse.Properties.Add("Item", ConvertEvent(repoResponse.Entity));
                        jsonResponse.Succeeded = true;
                    }
                        
                }
                catch (Exception ex)
                {
                    jsonResponse.Succeeded = false;
                    jsonResponse.Message = ex.GetInnermostException().Message;
                }

            }
            else
            {
                jsonResponse.Fail("Could not create event");
            }

            return Json(jsonResponse);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ViewResult EditEventAjax(int id)
        {
            EditEventViewModel vm = new EditEventViewModel
            {
                DateConverter = CmsContext.DateConverter,
                ContentProperties_Body_Html = new HtmlContentProperties(),
                ContentProperties_Body_ImageFile = new FileContentProperties
                {
                    RootFolder = "/Common/Cms/Images",
                    DefaultSubfolder = "EventImages",
                    MediaTypes = new string[] { "image/" }
                },
                ContentProperties_Body_DocumentFile = new FileContentProperties
                {
                    RootFolder = "/Common/Cms/Documents"
                },
                ContentProperties_Image = new ImageContentProperties
                {
                    ExactWidth = 180,
                    ExactHeight = 120
                },
                ContentProperties_ImageFile = new FileContentProperties
                {
                    RootFolder = "/Common/Cms/Images",
                    DefaultSubfolder = "EventImages",
                    MediaTypes = new string[] { "image/" }
                }
            };
            var repoResponse = Repository.Events.GetSingle(id);
            if (repoResponse.Succeeded)
            {
                vm.Event = repoResponse.Entity;
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult EditEventAjax(string id, string property, string newValue)
        {
            int ID = 0;
            if (int.TryParse(id, out ID))
            {
                try
                {
                    var entity = Repository.Events.GetSingle(ID).Entity;
                    EditInPlaceJsonResponse response = EditProperty(editID => entity, id, property, newValue, null, null, null);
                    return Json(response);
                }
                catch (Exception ex)
                {
                    return Json(new JsonResponse { Succeeded = false, Message = string.Format("An exception occurred: {0}", ex.Message) });
                }
            }
            else
            {
                return Json(new JsonResponse { Succeeded = false, Message = string.Format("Event ID {0} was not regognized.", id) });
            }
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult DeleteEventAjax(int id)
        {
            RepositoryResponse repoResponse = Repository.Events.Delete(id);
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("Event with ID {0} deleted.", id));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult FindEventsAjax(string term)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Events.FindForDisplay(term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries));
            if (repoResponse.Succeeded)
            {
                var items = ConvertEvents(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} calendar event(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult AllEventsAjax()
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Events.GetAll();
            if (repoResponse.Succeeded)
            {
                var items = ConvertEvents(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} calendar event(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        private IEnumerable<object> ConvertEvents(IEnumerable<Event> items)
        {
            return items.Select(a =>
            {
                return ConvertEvent(a);
            });
        }

        private object ConvertEvent(Event item)
        {
            return new
            {
                // generic and required by reusable JavaScript
                ID = item.EventID,
                TabTitleString = item.Title,
                DateCreated = CmsContext.DateConverter.Convert(item.DateCreated).FromUtc().ForCmsUser().ToString("g"),

                // specific to this object
                Title = item.Title,
                ImageURL = item.ImageURL,
                Description = item.Description,
                StartDate = item.StartDate.HasValue ? CmsContext.DateConverter.Convert(item.StartDate.Value).FromUtc().ForCmsUser().ToShortDateString() : string.Empty,
                EndDate = item.EndDate.HasValue ? CmsContext.DateConverter.Convert(item.EndDate.Value).FromUtc().ForCmsUser().ToShortDateString() : string.Empty,
            };
        }
    }
}