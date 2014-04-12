using Clickfarm.AppFramework.Responses;
using Clickfarm.AppFramework.Extensions;
using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using Clickfarm.Cms.Core;
using KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Tags;
using KCActorsTheatre.Announcements;
using KCActorsTheatre.MissionStories;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.Controllers
{
    public class TagsController : KCActorsTheatreAdminControllerBase
    {
        public TagsController(ICmsContext context) : base(context) { }

        #region Views

        public ActionResult General()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            vm.DescriptionSingular = "General Tag";
            vm.DescriptionPlural = "General Tags";
            vm.TagType = TagType.General;
            return View("Index", vm);
        }

        public ActionResult MissionStory()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            vm.DescriptionSingular = "Mission Story Tag";
            vm.DescriptionPlural = "Mission Story Tags";
            vm.TagType = TagType.MissionStory;
            return View("Index", vm);
        }

        public ActionResult Blog()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            vm.DescriptionSingular = "Blog Tag";
            vm.DescriptionPlural = "Blog Tags";
            vm.TagType = TagType.Blog;
            return View("Index", vm);
        }

        #endregion

        #region Tags

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult All(TagType tagType = TagType.General)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Tags.GetAllByType(tagType);
            if (repoResponse.Succeeded)
            {
                var items = ConvertTags(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} tag(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Find(string term, TagType tagType = TagType.General)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Tags.FindForDisplay(term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries), tagType);
            if (repoResponse.Succeeded)
            {
                var items = ConvertTags(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} tag(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Create(Tag tag)
        {
            // set some defaults
            tag.DateCreated = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                RepositoryResponse<Tag> repoResponse = Repository.Tags.New(tag);
                if (repoResponse.Succeeded)
                {
                    var jsonResponse = new JsonResponse
                    {
                        Succeeded = true,
                        Message = "New tag created.",

                    };
                    jsonResponse.Properties.Add("Item", ConvertTag(repoResponse.Entity));
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

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ViewResult Edit(int id)
        {
            EditViewModel vm = new EditViewModel
            {
                DateConverter = CmsContext.DateConverter,
            };
            var repoResponse = Repository.Tags.GetSingle(id);
            if (repoResponse.Succeeded)
            {
                vm.Tag = repoResponse.Entity;
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
                    var entity = Repository.Tags.GetSingle(ID).Entity;
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
                return Json(new JsonResponse { Succeeded = false, Message = string.Format("Tag ID {0} was not regognized.", id) });
            }
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Delete(int id)
        {
            RepositoryResponse repoResponse = Repository.Tags.Delete(id);
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("Tag with ID {0} deleted.", id));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        #endregion

        #region Widget Tagging

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult GetWidgetTags(int contentID)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Tags.GetWidgetTags(contentID);
            if (repoResponse.Succeeded)
            {
                var items = ConvertTags(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} tag(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        public JsonResult AddWidgetTag(int contentID, int tagID)
        {
            var jsonResponse = new JsonResponse();
            var repoResp = Repository.Tags.AddWidgetTag(contentID, tagID);
            if (repoResp.Succeeded)
            {
                var item = ConvertTag(repoResp.Entity);
                jsonResponse.Properties.Add("Item", item);
                jsonResponse.Succeed("added tag");
            }
            else
            {
                jsonResponse.Fail(repoResp.Message);
            }
            return Json(jsonResponse);
        }

        public JsonResult RemoveWidgetTag(int contentID, int tagID)
        {
            var jsonResponse = new JsonResponse();
            var repoResp = Repository.Tags.RemoveWidgetTag(contentID, tagID);
            if (repoResp.Succeeded)
            {
                var item = ConvertTag(repoResp.Entity);
                jsonResponse.Properties.Add("Item", item);
                jsonResponse.Succeed("removed tag");
            }
            else
            {
                jsonResponse.Fail(repoResp.Message);
            }
            return Json(jsonResponse);
        }

        #endregion

        #region Private Helpers

        private IEnumerable<object> ConvertTags(IEnumerable<Tag> tags)
        {
            return tags.Select(a =>
            {
                return ConvertTag(a);
            });
        }

        private object ConvertTag(Tag tag)
        {
            return new
            {
                // generic and required by reusable JavaScript
                ID = tag.TagID,
                TabTitleString = tag.Name,
                DateModified = tag.DateModified.HasValue ? CmsContext.DateConverter.Convert(tag.DateModified.Value).FromUtc().ForCmsUser().ToString("g") : "Never",

                // specific to this object
                Name = tag.Name,
                NumberOfAssociatedMissionStories = tag.MissionStories.Count(),
                NumberOfAssociatedAnnouncements = tag.Announcements.Where(p => p.AnnouncementType == AnnouncementType.Announcement).Count(),
                NumberOfAssociatedOfficialAnnouncements = tag.Announcements.Where(p => p.AnnouncementType == AnnouncementType.Official).Count(),
                NumberOfAssociatedResources = tag.Resources.Count,
                NumberOfAssociatedBlogPosts = tag.Posts.Count,

            };
        }

        #endregion

    }
}
