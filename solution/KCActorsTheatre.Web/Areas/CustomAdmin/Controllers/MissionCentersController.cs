using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Data;
using KCActorsTheatre.Locations;
using KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.MissionCenters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.Controllers
{
    public class MissionCentersController : KCActorsTheatreAdminControllerBase
    {
        public MissionCentersController(ICmsContext context) : base(context)
        {
        }

        public ActionResult Index()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            vm.MissionFieldList = Repository.MissionFields.GetAll().Entity.ToList().ConvertAll<SelectListItem>(c =>
            {
                SelectListItem item = new SelectListItem
                {
                    Text = c.Name,
                    Value = c.MissionFieldID.ToString(),
                };
                return item;
            });

            return View(vm);
        }

        #region Mission Centers

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Find(string term)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.MissionCenters.FindForDisplay(term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries));
            if (repoResponse.Succeeded)
            {
                var items = ConvertMissionCenters(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} Mission Center(s) found.", items.Count()));
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
            var repoResponse = Repository.MissionCenters.GetAll();
            if (repoResponse.Succeeded)
            {
                var items = ConvertMissionCenters(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} Mission Center(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Create(MissionCenter missionCenter)
        {
            // set defaults
            missionCenter.DateCreated = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                RepositoryResponse<MissionCenter> repoResponse = Repository.MissionCenters.New(missionCenter);
                if (repoResponse.Succeeded)
                {
                    var jsonResponse = new JsonResponse
                    {
                        Succeeded = true,
                        Message = "New mission center created.",

                    };
                    jsonResponse.Properties.Add("Item", ConvertMissionCenter(repoResponse.Entity));
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
            RepositoryResponse repoResponse = Repository.MissionCenters.Delete(id);
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("Mission Center with ID {0} deleted.", id));
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

                // set thumbnail image width and height
                ContentProperties_ThumbnailImage = new ImageContentProperties
                {
                    ExactWidth = 200,
                    ExactHeight = 200
                },
                // set thumbnail image folder properties and media types
                ContentProperties_ThumbnailImageFile = new FileContentProperties
                {
                    RootFolder = "/common/cms/images/missionCenters",
                    DefaultSubfolder = "",
                    MediaTypes = new string[] { "image/" }
                },
            };
            var repoResponse = Repository.MissionCenters.GetSingle(id);
            if (repoResponse.Succeeded)
            {
                vm.MissionCenter = repoResponse.Entity;
                //var roleDefinitions = 
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
                    var entity = Repository.MissionCenters.GetSingle(ID).Entity;
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
                return Json(new JsonResponse { Succeeded = false, Message = string.Format("MissionCenterID {0} was not regognized.", id) });
            }
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult DeleteRole(int missionCenterID, int roleDefinitionID)
        {
            RepositoryResponse repoResponse = Repository.MissionCenters.DeleteRole(missionCenterID, roleDefinitionID);
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("Role with ID {0} deleted.", roleDefinitionID));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }


        public JsonResult UpdateContactDisplayOrder(int missionCenterID, int[] roleDefinitionIDs)
        {
            var response = new JsonResponse();
            var repoResponse = Repository.MissionCenters.UpdateContactDisplayOrder(missionCenterID, roleDefinitionIDs);
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


        #endregion

        #region List Helpers

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult GetMissionFields()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var item in Repository.MissionFields.All().ToList())
            {
                dict.Add(item.MissionFieldID.ToString(), item.Name);
            }
            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult GetPhoneTypes()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (PhoneType s in EnumExtensions.GetMembers<PhoneType>())
            {
                dict.Add(s.ToString(), s.GetDescription());
            }
            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Private Helpers

        private IEnumerable<object> ConvertMissionCenters(IEnumerable<MissionCenter> items)
        {
            return items.Select(a =>
            {
                return ConvertMissionCenter(a);
            });
        }

        private object ConvertMissionCenter(MissionCenter item)
        {
            return new
            {
                // generic and required by reusable JavaScript
                ID = item.MissionCenterID,
                TabTitleString = item.Name,
                DateModified = item.DateModified.HasValue ? CmsContext.DateConverter.Convert(item.DateModified.Value).FromUtc().ForCmsUser().ToString("g") : "Never",

                // specific to this object
                Name = item.Name,
                MissionField = item.MissionField.Name,
            };
        }

        #endregion

    }
}