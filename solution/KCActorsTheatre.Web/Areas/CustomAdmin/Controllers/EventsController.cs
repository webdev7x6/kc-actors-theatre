using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Calendar;
using KCActorsTheatre.Data;
using KCActorsTheatre.Tags;
using KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Calendar;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.Controllers
{
    public class EventsController : KCActorsTheatreAdminControllerBase
    {
        public EventsController(ICmsContext context) : base(context) { }

        public ActionResult Index()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            vm.EventCategoryList = Repository.EventCategories.GetAll().Entity.ToList().ConvertAll<SelectListItem>(c =>
            {
                SelectListItem item = new SelectListItem
                {
                    Text = c.Name,
                    Value = c.EventCategoryID.ToString(),
                };
                return item;
            });
            return View("Index", vm);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Find(string term)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.CalendarEvents.Find(term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries));
            if (repoResponse.Succeeded)
            {
                var items = ConvertEvents(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult All()
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.CalendarEvents.GetAll();
            if (repoResponse.Succeeded)
            {
                var items = ConvertEvents(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Create(CalendarEvent calendarEvent)
        {
            // set some defaults
            calendarEvent.DateCreated = DateTime.UtcNow;
            calendarEvent.StartDate = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                RepositoryResponse<CalendarEvent> repoResponse = Repository.CalendarEvents.New(calendarEvent);
                if (repoResponse.Succeeded)
                {
                    var jsonResponse = new JsonResponse
                    {
                        Succeeded = true,
                        Message = "New item created.",

                    };
                    jsonResponse.Properties.Add("Item", ConvertEvent(repoResponse.Entity));
                    return Json(jsonResponse);
                }
                else
                {
                    return Json(new JsonResponse
                    {
                        Succeeded = false,
                        Message = repoResponse.Message
                    });
                }
            }
            else
            {
                return Json(new JsonResponse
                {
                    Succeeded = false,
                    Message = string.Join("Error", ModelState.ErrorMessages())
                });
            }
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Delete(int id)
        {
            RepositoryResponse repoResponse = Repository.CalendarEvents.Delete(id);
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("Item with ID {0} deleted.", id));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ViewResult Edit(int id)
        {
            EditViewModel vm = new EditViewModel
            {
                DateConverter = CmsContext.DateConverter,
            };
            var repoResponse = Repository.CalendarEvents.GetSingle(id);
            if (repoResponse.Succeeded)
            {
                vm.CalendarEvent = repoResponse.Entity;
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Edit(string id, string property, string newValue)
        {
            int ID = 0;
            if (int.TryParse(id, out ID))
            {
                try
                {
                    var entity = Repository.CalendarEvents.GetSingle(ID).Entity;
                    entity.DateModified = DateTime.UtcNow;
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
                return Json(new JsonResponse { Succeeded = false, Message = string.Format("Item ID {0} was not regognized.", id) });
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult GetEventCategoriesAjax()
        {
            OrderedDictionary orderedDictionary = new OrderedDictionary();

            var items = Repository.EventCategories
                .All()
                .ToList()
                ;

            foreach (var item in items)
                orderedDictionary.Add(item.EventCategoryID.ToString(), string.Format("{0}: {1}", item.Name, item.Name));

            return Json(orderedDictionary, JsonRequestBehavior.AllowGet);
        }


        #region Tags

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult FindTags(string term)
        {
            var termsSplit = term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            var tags = Repository.Tags.Find(termsSplit, TagType.General);
            return Json(Tags(tags.Entity));
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult AllTags()
        {
            var tags = Repository.Tags
                .All()
                .Where(p => p.TagType == TagType.General);

            return Json(Tags(tags));
        }


        public JsonResult AddTag(int id, int tagID)
        {
            var repoResp = Repository.CalendarEvents.AddTag(id, tagID);
            return Json(new JsonResponse
            {
                Succeeded = repoResp.Succeeded,
                Message = repoResp.Message
            });
        }

        public JsonResult RemoveTag(int id, int tagID)
        {
            var repoResp = Repository.CalendarEvents.RemoveTag(id, tagID);
            return Json(new JsonResponse
            {
                Succeeded = repoResp.Succeeded,
                Message = repoResp.Message
            });
        }

        #endregion

        #region Private Helpers

        private JsonResponse Tags(IEnumerable<Tag> tags)
        {
            var resp = new JsonResponse();
            resp.Properties.Add("Tags", tags);
            resp.Succeed(string.Format("{0} tags found.", tags.Count()));
            return resp;
        }

        private IEnumerable<object> ConvertEvents(IEnumerable<CalendarEvent> calendarEvents)
        {
            return calendarEvents.Select(a =>
            {
                return ConvertEvent(a);
            });
        }

        private object ConvertEvent(CalendarEvent calendarEvent)
        {
            return new
            {
                // generic and required by reusable JavaScript
                ID = calendarEvent.CalendarEventID,
                TabTitleString = calendarEvent.Title,
                DateModified = calendarEvent.DateModified.HasValue ? CmsContext.DateConverter.Convert(calendarEvent.DateModified.Value).FromUtc().ForCmsUser().ToString("g") : "Never",

                // specific to this object
                Title = calendarEvent.Title,
                IsPublished = IsPublished(calendarEvent) ? "Yes" : "No",
                EventCategory = calendarEvent.EventCategory != null ? calendarEvent.EventCategory.Name : "",
            };
        }

        private bool IsPublished(CalendarEvent calendarEvent)
        {
            return calendarEvent.StartDate <= DateTime.UtcNow;
        }

        #endregion
    }
}