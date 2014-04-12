using Clickfarm.AppFramework.Responses;
using Clickfarm.AppFramework.Extensions;
using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using Clickfarm.Cms.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Blogs;
using KCActorsTheatre.Blogs;
using KCActorsTheatre.Tags;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.Controllers
{
    public class BlogsController : KCActorsTheatreAdminControllerBase
    {
        public BlogsController(ICmsContext context) : base(context) { }

        #region Views

        public ActionResult Evangelist()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            vm.DescriptionSingular = "Evangelist Blog Post";
            vm.DescriptionPlural = "Evangelist Blog Posts";
            vm.BlogType = BlogType.Evangelist;
            return View("Index", vm);
        }

        public ActionResult DailyBread()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            vm.DescriptionSingular = "Daily Bread Blog Post";
            vm.DescriptionPlural = "Daily Bread Blog Posts";
            vm.BlogType = BlogType.DailyBread;
            return View("Index", vm);
        }

        #endregion

        #region Posts

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult AllByType(BlogType blogType)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Posts.GetAllByType(blogType);
            if (repoResponse.Succeeded)
            {
                var items = ConvertPosts(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} blog post(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult FindByType(string term, BlogType blogType)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Posts.FindByTypeForDisplay(term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries), blogType);
            if (repoResponse.Succeeded)
            {
                var items = ConvertPosts(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} organization(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Create(Post post)
        {
            // set some defaults
            post.DateCreated = DateTime.UtcNow;
            post.Status = PostStatus.Draft;

            if (ModelState.IsValid)
            {
                RepositoryResponse<Post> repoResponse = Repository.Posts.New(post);
                if (repoResponse.Succeeded)
                {
                    var jsonResponse = new JsonResponse
                    {
                        Succeeded = true,
                        Message = "New post created.",

                    };
                    jsonResponse.Properties.Add("Item", ConvertPost(repoResponse.Entity));
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
            var repoResponse = Repository.Posts.GetSingle(id);
            if (repoResponse.Succeeded)
            {
                vm.Post = repoResponse.Entity;
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
                    var entity = Repository.Posts.GetSingle(ID).Entity;
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
                return Json(new JsonResponse { Succeeded = false, Message = string.Format("PostID {0} was not regognized.", id) });
            }
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Delete(int id)
        {
            RepositoryResponse repoResponse = Repository.Posts.Delete(id);
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("Post with ID {0} deleted.", id));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult GetStatuses()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (PostStatus s in EnumExtensions.GetMembers<PostStatus>())
            {
                dict.Add(s.ToString(), s.GetDescription());
            }
            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Tags

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult FindTags(string term)
        {
            var termsSplit = term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            var tags = Repository.Tags.Find(termsSplit, TagType.Blog);
            return Json(Tags(tags.Entity));
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult AllTags()
        {
            var tags = Repository.Tags
                .All()
                .Where(p => p.TagType == TagType.Blog);

            return Json(Tags(tags));
        }


        public JsonResult AddTag(int id, int tagID)
        {
            var repoResp = Repository.Posts.AddTag(id, tagID);
            return Json(new JsonResponse
            {
                Succeeded = repoResp.Succeeded,
                Message = repoResp.Message
            });
        }

        public JsonResult RemoveTag(int id, int tagID)
        {
            var repoResp = Repository.Posts.RemoveTag(id, tagID);
            return Json(new JsonResponse
            {
                Succeeded = repoResp.Succeeded,
                Message = repoResp.Message
            });
        }

        #endregion

        #region Categories

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult FindCategories(string term)
        {
            var termsSplit = term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            var repoResponse = Repository.PostCategories.Find(termsSplit);
            return Json(Categories(repoResponse.Entity));
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult AllCategories()
        {
            var categories = Repository.PostCategories
                .All()
                ;

            return Json(Categories(categories));
        }


        public JsonResult AddCategory(int id, int categoryID)
        {
            var repoResponse = Repository.Posts.AddCategory(id, categoryID);
            return Json(new JsonResponse
            {
                Succeeded = repoResponse.Succeeded,
                Message = repoResponse.Message
            });
        }

        public JsonResult RemoveCategory(int id, int categoryID)
        {
            var repoResponse = Repository.Posts.RemoveCategory(id, categoryID);
            return Json(new JsonResponse
            {
                Succeeded = repoResponse.Succeeded,
                Message = repoResponse.Message
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

        private JsonResponse Categories(IEnumerable<PostCategory> postCategories)
        {
            var jsonResponse = new JsonResponse();
            jsonResponse.Properties.Add("Categories", postCategories);
            jsonResponse.Succeed(string.Format("{0} categories found.", postCategories.Count()));
            return jsonResponse;
        }

        private IEnumerable<object> ConvertPosts(IEnumerable<Post> items)
        {
            return items.Select(a =>
            {
                return ConvertPost(a);
            });
        }

        private object ConvertPost(Post item)
        {
            return new
            {
                // generic and required by reusable JavaScript
                ID = item.PostID,
                TabTitleString = item.Title,
                DateModified = item.DateModified.HasValue ? CmsContext.DateConverter.Convert(item.DateModified.Value).FromUtc().ForCmsUser().ToString("g") : "Never",

                // specific to this object
                Title = item.Title,
                Status = item.Status.GetDescription(),
            };
        }

        #endregion

    }
}
