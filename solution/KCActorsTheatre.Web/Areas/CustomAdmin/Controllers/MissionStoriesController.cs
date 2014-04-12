using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.MissionStories;
using KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.MissionStories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCActorsTheatre.Tags;
using System.Collections.Specialized;
using KCActorsTheatre.Resources;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.Controllers
{
    public class MissionStoriesController : KCActorsTheatreAdminControllerBase
    {
        public MissionStoriesController(ICmsContext context) : base(context) { }

        public ActionResult Index()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            return View(vm);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Find(string term)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.MissionStories.FindForDisplay(term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries));
            if (repoResponse.Succeeded)
            {
                var items = ConvertMissionStories(repoResponse.Entity);
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
            var repoResponse = Repository.MissionStories.GetAll();
            if (repoResponse.Succeeded)
            {
                var items = ConvertMissionStories(repoResponse.Entity);
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
        public JsonResult Create(MissionStory missionStory)
        {
            // set some defaults
            missionStory.DateCreated = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                RepositoryResponse<MissionStory> repoResponse = Repository.MissionStories.New(missionStory);
                if (repoResponse.Succeeded)
                {
                    var jsonResponse = new JsonResponse
                    {
                        Succeeded = true,
                        Message = "New item created.",

                    };
                    jsonResponse.Properties.Add("Item", ConvertMissionStory(repoResponse.Entity));
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
            RepositoryResponse repoResponse = Repository.MissionStories.Delete(id);
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
            var repoResponse = Repository.MissionStories.GetSingle(id);
            if (repoResponse.Succeeded)
            {
                vm.MissionStory = repoResponse.Entity;
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
                    var entity = Repository.MissionStories.GetSingle(ID).Entity;
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

        #region Tags

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult FindTags(string term)
        {
            var termsSplit = term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            var tags = Repository.Tags.Find(termsSplit, TagType.MissionStory);
            return Json(Tags(tags.Entity));
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult AllTags()
        {
            var tags = Repository.Tags
                .All()
                .Where(p => p.TagType == TagType.MissionStory);

            return Json(Tags(tags));
        }


        public JsonResult AddTag(int id, int tagID)
        {
            var repoResp = Repository.MissionStories.AddTag(id, tagID);
            return Json(new JsonResponse
            {
                Succeeded = repoResp.Succeeded,
                Message = repoResp.Message
            });
        }

        public JsonResult RemoveTag(int id, int tagID)
        {
            var repoResp = Repository.MissionStories.RemoveTag(id, tagID);
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
                .Where(p => p.ResourceType == ResourceType.Audio || p.ResourceType == ResourceType.Video)
                .ToList()
                ;

            foreach (var item in items)
                orderedDictionary.Add(item.ResourceID.ToString(), string.Format("{0}: {1}", item.ResourceType.GetDescription(), item.Title));

            return Json(orderedDictionary, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Used in HomePageMissionStoriesWidgetContent view

        public JsonResult UpdateMissionStoriesDisplayOrder(int[] missionStoryIDs)
        {
            var response = new JsonResponse();
            var repoResponse = Repository.MissionStories.UpdateMissionStoriesDisplayOrder(missionStoryIDs);
            if (repoResponse.Succeeded)
            {
                response.Succeed(repoResponse.Message);
            }
            else
            {
                response.Fail(repoResponse.Message);
            }

            return Json(response);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult GetHomePageMissionStories()
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.MissionStories.GetHomePageMissionStories();
            if (repoResponse.Succeeded)
            {
                var items = ConvertMissionStories(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
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

        private IEnumerable<object> ConvertMissionStories(IEnumerable<MissionStory> missionStories)
        {
            return missionStories.Select(a =>
            {
                return ConvertMissionStory(a);
            });
        }

        private object ConvertMissionStory(MissionStory missionStory)
        {
            return new
            {
                // generic and required by reusable JavaScript
                ID = missionStory.MissionStoryID,
                TabTitleString = missionStory.Title,
                DateModified = missionStory.DateModified.HasValue ? CmsContext.DateConverter.Convert(missionStory.DateModified.Value).FromUtc().ForCmsUser().ToString("g") : "Never",

                // specific to this object
                Title = missionStory.Title,
                IsPublished = IsPublished(missionStory) ? "Yes" : "No",
            };
        }

        private bool IsPublished(MissionStory missionStory)
        {
            return missionStory.DatePublished.HasValue
                && missionStory.DatePublished.Value <= DateTime.UtcNow
            ;
        }

        #endregion
    }
}