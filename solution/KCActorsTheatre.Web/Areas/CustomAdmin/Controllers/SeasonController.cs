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
using KCActorsTheatre.Contract;
using KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Season;
using KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Show;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.Controllers
{
    public class SeasonController : KCActorsTheatreAdminControllerBase
    {
        public SeasonController(ICmsContext context) : base(context) { }

        public ViewResult Index()
        {
            var model = new SeasonViewModel();
            model.Init(CmsContext);
            return View(model);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult CreateSeasonAjax(SeasonInfo season)
        {
            JsonResponse jsonResponse = new JsonResponse();

            season.DateCreated = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                try
                {
                    var repoResponse = Repository.Seasons.New(season);

                    if (repoResponse.Succeeded && repoResponse.Entity != null)
                    {
                        jsonResponse.Properties.Add("Item", ConvertSeason(repoResponse.Entity));
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
        public ViewResult EditSeasonAjax(int id)
        {
            var vm = new EditSeasonViewModel
            {
                DateConverter = CmsContext.DateConverter,
                ContentProperties_Body_Html = new HtmlContentProperties(),
                ContentProperties_Body_ImageFile = new FileContentProperties
                {
                    RootFolder = "/Common/Cms/Images",
                    DefaultSubfolder = "SeasonImages",
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
                    DefaultSubfolder = "SeasonImages",
                    MediaTypes = new string[] { "image/" }
                }
            };
            var repoResponse = Repository.Seasons.GetSingle(id);
            if (repoResponse.Succeeded)
            {
                vm.Season = repoResponse.Entity;
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult EditSeasonAjax(string id, string property, string newValue)
        {
            int ID = 0;
            if (int.TryParse(id, out ID))
            {
                try
                {
                    var entity = Repository.Seasons.GetSingle(ID).Entity;
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
                return Json(new JsonResponse { Succeeded = false, Message = string.Format("item ID {0} was not regognized.", id) });
            }
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult DeleteSeasonAjax(int id)
        {
            RepositoryResponse repoResponse = Repository.Seasons.Delete(id);
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("item with ID {0} deleted.", id));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult FindSeasonsAjax(string term)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Seasons.FindForDisplay(term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries));
            if (repoResponse.Succeeded)
            {
                var items = ConvertSeasons(repoResponse.Entity);
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
        public JsonResult AllSeasonsAjax()
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Seasons.GetAll();
            if (repoResponse.Succeeded)
            {
                var items = ConvertSeasons(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} items(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        private IEnumerable<object> ConvertSeasons(IEnumerable<SeasonInfo> items)
        {
            return items.Select(ConvertSeason);
        }

        private object ConvertSeason(SeasonInfo item)
        {
            return new
            {
                // generic and required by reusable JavaScript
                ID = item.SeasonID,
                TabTitleString = item.Title,
                DateCreated = CmsContext.DateConverter.Convert(item.DateCreated).FromUtc().ForCmsUser().ToString("g"),

                // specific to this object
                Title = item.Title
            };
        }
    }
}