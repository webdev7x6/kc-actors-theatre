using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Blogs;
using KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.PostCategories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.Controllers
{
    public class PostCategoriesController : KCActorsTheatreAdminControllerBase
    {
        public PostCategoriesController(ICmsContext context) : base(context) { }

        public ActionResult Index()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            return View(vm);
        }


        #region PostCategories

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Find(string term)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.PostCategories.FindForDisplay(term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries));
            if (repoResponse.Succeeded)
            {
                var items = ConvertPostCategories(repoResponse.Entity);
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
            var repoResponse = Repository.PostCategories.GetAll();
            if (repoResponse.Succeeded)
            {
                var items = ConvertPostCategories(repoResponse.Entity);
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
        public JsonResult Create(PostCategory postCategory)
        {
            // set defaults
            postCategory.DateCreated = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                RepositoryResponse<PostCategory> repoResponse = Repository.PostCategories.New(postCategory);
                if (repoResponse.Succeeded)
                {
                    var jsonResponse = new JsonResponse
                    {
                        Succeeded = true,
                        Message = "New post category created.",

                    };
                    jsonResponse.Properties.Add("Item", ConvertPostCategory(repoResponse.Entity));
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
            RepositoryResponse repoResponse = Repository.PostCategories.Delete(id);
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("PostCategory with ID {0} deleted.", id));
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
            var repoResponse = Repository.PostCategories.GetSingle(id);
            if (repoResponse.Succeeded)
            {
                vm.PostCategory = repoResponse.Entity;
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
                    var entity = Repository.PostCategories.GetSingle(ID).Entity;
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
                return Json(new JsonResponse { Succeeded = false, Message = string.Format("PostCategoryID {0} was not regognized.", id) });
            }
        }

        #endregion

        #region Private Helpers

        private IEnumerable<object> ConvertPostCategories(IEnumerable<PostCategory> postCategories)
        {
            return postCategories.Select(a =>
            {
                return ConvertPostCategory(a);
            });
        }

        private object ConvertPostCategory(PostCategory postCategory)
        {
            return new
            {
                // generic and required by reusable JavaScript
                ID = postCategory.PostCategoryID,
                TabTitleString = postCategory.Name,
                DateModified = postCategory.DateModified.HasValue ? CmsContext.DateConverter.Convert(postCategory.DateModified.Value).FromUtc().ForCmsUser().ToString("g") : "Never",

                // specific to this object
                Name = postCategory.Name,
            };
        }

        #endregion

    }
}