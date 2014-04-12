using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Resources;
using KCActorsTheatre.Tags;
using KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.Controllers
{
    public class ResourcesController : KCActorsTheatreAdminControllerBase
    {
        public ResourcesController(ICmsContext context) : base(context) { }

        #region Views

        public ActionResult Audio()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            vm.DescriptionSingular = "Audio Resource";
            vm.DescriptionPlural = "Audio Resources";
            vm.ResourceType = ResourceType.Audio;
            vm.TypeofResource = typeof(AudioResource);
            return View("Index", vm);
        }
        public ActionResult Document()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            vm.DescriptionSingular = "Document Resource";
            vm.DescriptionPlural = "Document Resources";
            vm.ResourceType = ResourceType.Document;
            vm.TypeofResource = typeof(DocumentResource);
            return View("Index", vm);
        }
        public ActionResult Media()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            vm.DescriptionSingular = "CD/DVD Resource";
            vm.DescriptionPlural = "CD/DVD Resources";
            vm.ResourceType = ResourceType.Media;
            vm.TypeofResource = typeof(MediaResource);
            return View("Index", vm);
        }
        public ActionResult Product()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            vm.DescriptionSingular = "Product Resource";
            vm.DescriptionPlural = "Product Resources";
            vm.ResourceType = ResourceType.Product;
            vm.TypeofResource = typeof(ProductResource);
            return View("Index", vm);
        }
        public ActionResult Publication()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            vm.DescriptionSingular = "Publication Resource";
            vm.DescriptionPlural = "Publication Resources";
            vm.ResourceType = ResourceType.Publication;
            vm.TypeofResource = typeof(PublicationResource);
            return View("Index", vm);
        }
        public ActionResult Presentation()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            vm.DescriptionSingular = "Slideshare Presentation Resource";
            vm.DescriptionPlural = "Slideshare Presentation Resources";
            vm.ResourceType = ResourceType.Presentation;
            vm.TypeofResource = typeof(PresentationResource);
            return View("Index", vm);
        }
        public ActionResult Slideshow()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            vm.DescriptionSingular = "Slideshow Resource";
            vm.DescriptionPlural = "Slideshow Resources";
            vm.ResourceType = ResourceType.Slideshow;
            vm.TypeofResource = typeof(SlideshowResource);
            return View("Index", vm);
        }
        public ActionResult Video()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            vm.DescriptionSingular = "Video Resource";
            vm.DescriptionPlural = "Video Resources";
            vm.ResourceType = ResourceType.Video;
            vm.TypeofResource = typeof(VideoResource);
            return View("Index", vm);
        }

        #endregion Views

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Find(string term, ResourceType resourceType)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Resources.FindForDisplay(term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries), resourceType);
            if (repoResponse.Succeeded)
            {
                var items = ConvertResources(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} resource(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult All(ResourceType resourceType)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Resources.GetAllByType(resourceType);
            if (repoResponse.Succeeded)
            {
                var items = ConvertResources(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} resource(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Create(Resource resource, string TypeofResource)
        {
            Resource newResource = Activator.CreateInstance(Type.GetType(TypeofResource)) as Resource;

            // set some defaults
            newResource.DateCreated = DateTime.UtcNow;
            newResource.Language = Language.English;

            // copy from modelstate object
            newResource.Title = resource.Title;

            if (ModelState.IsValid)
            {
                RepositoryResponse<Resource> repoResponse = Repository.Resources.New(newResource);
                if (repoResponse.Succeeded)
                {
                    var jsonResponse = new JsonResponse
                    {
                        Succeeded = true,
                        Message = "New resource created.",

                    };
                    jsonResponse.Properties.Add("Item", ConvertResource(repoResponse.Entity));
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
            RepositoryResponse repoResponse = Repository.Resources.Delete(id);
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("Resource with ID {0} deleted.", id));
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
            var repoResponse = Repository.Resources.GetSingle(id);
            if (repoResponse.Succeeded)
            {
                vm.Resource = repoResponse.Entity;
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
                    var entity = Repository.Resources.GetSingle(ID).Entity;
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
                return Json(new JsonResponse { Succeeded = false, Message = string.Format("Resource ID {0} was not regognized.", id) });
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult GetLanguagesAjax()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (Language s in EnumExtensions.GetMembers<Language>())
            {
                dict.Add(s.ToString(), s.GetDescription());
            }
            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult GetResourceTypesAjax()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (ResourceType s in EnumExtensions.GetMembers<ResourceType>())
            {
                dict.Add(s.ToString(), s.GetDescription());
            }
            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult GetVideoTypesAjax()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (VideoType s in EnumExtensions.GetMembers<VideoType>())
            {
                dict.Add(s.ToString(), s.GetDescription());
            }
            return Json(dict, JsonRequestBehavior.AllowGet);
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
            var tags = Repository.Tags.All().Where(p => p.TagType == TagType.General);
            return Json(Tags(tags));
        }

        private JsonResponse Tags(IEnumerable<Tag> tags)
        {
            var resp = new JsonResponse();
            resp.Properties.Add("Tags", tags);
            resp.Succeed(string.Format("{0} tags found.", tags.Count()));
            return resp;
        }

        public JsonResult AddTag(int id, int tagID)
        {
            var repoResp = Repository.Resources.AddTag(id, tagID);
            return Json(new JsonResponse
            {
                Succeeded = repoResp.Succeeded,
                Message = repoResp.Message
            });
        }

        public JsonResult RemoveTag(int id, int tagID)
        {
            var repoResp = Repository.Resources.RemoveTag(id, tagID);
            return Json(new JsonResponse
            {
                Succeeded = repoResp.Succeeded,
                Message = repoResp.Message
            });
        }


        #endregion

        #region Private Helpers

        private IEnumerable<object> ConvertResources(IEnumerable<Resource> resources)
        {
            return resources.Select(a =>
            {
                return ConvertResource(a);
            });
        }

        private object ConvertResource(Resource resource)
        {
            return new
            {
                // generic and required by reusable JavaScript
                ID = resource.ResourceID,
                TabTitleString = resource.Title,
                DateModified = resource.DateModified.HasValue ? CmsContext.DateConverter.Convert(resource.DateModified.Value).FromUtc().ForCmsUser().ToString("g") : "Never",

                // specific to this object
                Title = resource.Title,
                Description = resource.Description,
                IsPublished = IsResourcePublished(resource) ? "Yes" : "No",
                ResourceType = EnumExtensions.GetDescription(resource.ResourceType),
                Language = EnumExtensions.GetDescription(resource.Language),
            };
        }

        private bool IsResourcePublished (Resource resource)
        {
            return resource.DatePublished.HasValue && resource.DatePublished.Value > CmsContext.Now();
        }

        #endregion
    }
}