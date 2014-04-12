using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Announcements;
using KCActorsTheatre.Resources;
using KCActorsTheatre.Tags;
using KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Announcements;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;


namespace KCActorsTheatre.Web.Areas.CustomAdmin.Controllers
{
    public class AnnouncementsController : KCActorsTheatreAdminControllerBase
    {
        public AnnouncementsController(ICmsContext context) : base(context) { }

        #region Views for Article Types

        public ActionResult Index()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);

            vm.AnnouncementType = AnnouncementType.Announcement;
            vm.DescriptionSingular = "Announcement";
            vm.DescriptionPlural = "Announcements";

            return View("Index", vm);
        }

        public ActionResult Official()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);

            vm.AnnouncementType = AnnouncementType.Official;
            vm.DescriptionSingular = "Official Notification";
            vm.DescriptionPlural = "Official Notifications";

            return View("Index", vm);
        }

        #endregion Views for Article Types
        
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Find(string term, AnnouncementType type)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Announcements.FindForDisplay(term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries), type);
            if (repoResponse.Succeeded)
            {
                var items = ConvertAnnouncements((IEnumerable<Announcement>)repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} announcements(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult All(AnnouncementType type)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Announcements.GetAllByType(type);
            if (repoResponse.Succeeded)
            {
                var items = ConvertAnnouncements((IEnumerable<Announcement>)repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} announcements(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Create(Announcement announcement)
        {
            // set some defaults
            announcement.DateCreated = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                RepositoryResponse<Announcement> repoResponse = Repository.Announcements.New(announcement);
                if (repoResponse.Succeeded)
                {
                    var jsonResponse = new JsonResponse
                    {
                        Succeeded = true,
                        Message = "New announcement created.",

                    };
                    jsonResponse.Properties.Add("Item", ConvertAnnouncement(repoResponse.Entity));
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
            RepositoryResponse repoResponse = Repository.Announcements.Delete(id);
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("Announcement with ID {0} deleted.", id));
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
            var repoResponse = Repository.Announcements.GetSingle(id);
            if (repoResponse.Succeeded)
            {
                vm.Announcement = (Announcement)repoResponse.Entity;
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
                    var entity = Repository.Announcements.GetSingle(ID).Entity;
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
                return Json(new JsonResponse { Succeeded = false, Message = string.Format("Announcement ID {0} was not regognized.", id) });
            }
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
            var repoResp = Repository.Announcements.AddTag(id, tagID);
            return Json(new JsonResponse
            {
                Succeeded = repoResp.Succeeded,
                Message = repoResp.Message
            });
        }

        public JsonResult RemoveTag(int id, int tagID)
        {
            var repoResp = Repository.Announcements.RemoveTag(id, tagID);
            return Json(new JsonResponse
            {
                Succeeded = repoResp.Succeeded,
                Message = repoResp.Message
            });
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult GetResourcesAjax()
        {
            OrderedDictionary orderedDictionary = new OrderedDictionary();

            var items = Repository.Resources
                .All()
                .OrderBy(p => p.ResourceType.GetDescription())
                .ThenBy(p => p.Title)
                .ToList()
                ;

            foreach (var item in items)
                orderedDictionary.Add(item.ResourceID.ToString(), string.Format("{0}: {1}", item.ResourceType.GetDescription(), item.Title));

            return Json(orderedDictionary, JsonRequestBehavior.AllowGet);
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

        private IEnumerable<object> ConvertAnnouncements(IEnumerable<Announcement> announcements)
        {
            return announcements.Select(a =>
            {
                return ConvertAnnouncement(a);
            });
        }

        private object ConvertAnnouncement(Announcement announcement)
        {
            return new
            {
                // generic and required by reusable JavaScript
                ID = announcement.AnnouncementID,
                TabTitleString = announcement.Title,
                DateModified = announcement.DateModified.HasValue ? CmsContext.DateConverter.Convert(announcement.DateModified.Value).FromUtc().ForCmsUser().ToString("g") : "Never",

                // specific to this object
                Title = announcement.Title,
                IsPublished = IsPublished(announcement) ? "Yes" : "No",
            };
        }

        private bool IsPublished(Announcement announcement)
        {
            return announcement.DatePublished.HasValue
                && announcement.DatePublished.Value <= DateTime.UtcNow
                && (
                    (announcement.DateExpired.HasValue 
                    && announcement.DateExpired.Value >= DateTime.UtcNow)
                    ||
                    !announcement.DateExpired.HasValue
                )
            ;
        }

        #endregion
    }
}