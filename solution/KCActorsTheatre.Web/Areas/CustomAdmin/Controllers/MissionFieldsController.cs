using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Data;
using KCActorsTheatre.Locations;
using KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.MissionFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.Controllers
{
    public class MissionFieldsController : KCActorsTheatreAdminControllerBase
    {
        public MissionFieldsController(ICmsContext context) : base(context)
        {
        }

        public ActionResult Index()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            return View(vm);
        }

        #region Mission Fields

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Find(string term)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.MissionFields.FindForDisplay(term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries));
            if (repoResponse.Succeeded)
            {
                var items = ConvertMissionFields(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} Mission Fields(s) found.", items.Count()));
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
            var repoResponse = Repository.MissionFields.GetAll();
            if (repoResponse.Succeeded)
            {
                var items = ConvertMissionFields(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} Mission Field(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Create(MissionField missionField)
        {
            // set defaults
            missionField.DateCreated = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                RepositoryResponse<MissionField> repoResponse = Repository.MissionFields.New(missionField);
                if (repoResponse.Succeeded)
                {
                    var jsonResponse = new JsonResponse
                    {
                        Succeeded = true,
                        Message = "New mission field created.",

                    };
                    jsonResponse.Properties.Add("Item", ConvertMissionField(repoResponse.Entity));
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
            RepositoryResponse repoResponse = Repository.MissionFields.Delete(id);
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("Mission Field with ID {0} deleted.", id));
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
            var repoResponse = Repository.MissionFields.GetSingle(id);
            if (repoResponse.Succeeded)
            {
                vm.MissionField = repoResponse.Entity;
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
                    var entity = Repository.MissionFields.GetSingle(ID).Entity;
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
                return Json(new JsonResponse { Succeeded = false, Message = string.Format("MissionFieldID {0} was not regognized.", id) });
            }
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult DeleteRole(int missionFieldID, int roleDefinitionID)
        {
            RepositoryResponse repoResponse = Repository.MissionFields.DeleteRole(missionFieldID, roleDefinitionID);
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


        public JsonResult UpdateContactDisplayOrder(int missionFieldID, int[] roleDefinitionIDs)
        {
            var response = new JsonResponse();
            var repoResponse = Repository.MissionFields.UpdateContactDisplayOrder(missionFieldID, roleDefinitionIDs);
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

        #region Private Helpers

        private IEnumerable<object> ConvertMissionFields(IEnumerable<MissionField> items)
        {
            return items.Select(a =>
            {
                return ConvertMissionField(a);
            });
        }

        private object ConvertMissionField(MissionField item)
        {
            return new
            {
                // generic and required by reusable JavaScript
                ID = item.MissionFieldID,
                TabTitleString = item.Name,
                DateModified = item.DateModified.HasValue ? CmsContext.DateConverter.Convert(item.DateModified.Value).FromUtc().ForCmsUser().ToString("g") : "Never",

                // specific to this object
                Name = item.Name,
                NumberOfAssociatedMissionCenters = item.MissionCenters.Count(),
            };
        }

        #endregion

    }
}