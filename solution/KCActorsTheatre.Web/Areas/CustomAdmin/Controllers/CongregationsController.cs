using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Locations;
using KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Congregations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.Controllers
{
    public class CongregationsController : KCActorsTheatreAdminControllerBase
    {
        public CongregationsController(ICmsContext context) : base(context) { }

        public ActionResult Index()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            vm.MissionCenterList = Repository.MissionCenters.GetAll().Entity.ToList().ConvertAll<SelectListItem>(c =>
            {
                SelectListItem item = new SelectListItem
                {
                    Text = c.Name,
                    Value = c.MissionCenterID.ToString(),
                };
                return item;
            });
            return View(vm);
        }

        #region Congregation Contacts

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult CreateCongregationContactAjax(CongregationContact congregationContact)
        {
            // set defaults
            congregationContact.DateCreated = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                RepositoryResponse<CongregationContact> repoResponse = Repository.CongregationContacts.New(congregationContact);
                if (repoResponse.Succeeded)
                {
                    var jsonResponse = new JsonResponse
                    {
                        Succeeded = true,
                        Message = "New congregation contact created.",

                    };
                    jsonResponse.Properties.Add("CongregationContact", repoResponse.Entity);
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
        public JsonResult DeleteCongregationContactAjax(int id)
        {
            RepositoryResponse repoResponse = Repository.CongregationContacts.Delete(id);
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("Congregation Contact with ID {0} deleted.", id));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult EditContactInPlace(string contactID, string property, string newValue)
        {
            int ID = 0;
            if (int.TryParse(contactID, out ID))
            {
                try
                {
                    var entity = Repository.CongregationContacts.GetSingle(ID).Entity;
                    EditInPlaceJsonResponse response = EditProperty(editID => entity, contactID, property, newValue, null, null, null);
                    return Json(response);
                }
                catch (Exception ex)
                {
                    return Json(new JsonResponse { Succeeded = false, Message = string.Format("An exception occurred: {0}", ex.Message) });
                }
            }
            else
            {
                return Json(new JsonResponse { Succeeded = false, Message = string.Format("CongregationContactID {0} was not regognized.", contactID) });
            }
        }

        public JsonResult UpdateContactDisplayOrder(int[] contactIDs)
        {
            var response = new JsonResponse();
            var repoResponse = Repository.CongregationContacts.UpdateContactDisplayOrder(contactIDs);
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

        #region Congregations

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Find(string term)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Congregations.FindForDisplay(term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries));
            if (repoResponse.Succeeded)
            {
                var items = ConvertCongregations(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} congregation(s) found.", items.Count()));
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
            var repoResponse = Repository.Congregations.GetAll();
            if (repoResponse.Succeeded)
            {
                var items = ConvertCongregations(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} congregation(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Create(Congregation congregation)
        {
            // set defaults
            congregation.DateCreated = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                RepositoryResponse<Congregation> repoResponse = Repository.Congregations.New(congregation);
                if (repoResponse.Succeeded)
                {
                    var jsonResponse = new JsonResponse
                    {
                        Succeeded = true,
                        Message = "New congregation created.",

                    };
                    jsonResponse.Properties.Add("Item", ConvertCongregation(repoResponse.Entity));
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
            RepositoryResponse repoResponse = Repository.Congregations.Delete(id);
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("Congregation with ID {0} deleted.", id));
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
                    RootFolder = "/common/cms/images/congregation",
                    DefaultSubfolder = "congregation",
                    MediaTypes = new string[] { "image/" }
                },
            };
            var repoResponse = Repository.Congregations.GetSingle(id);
            if (repoResponse.Succeeded)
            {
                vm.Congregation = repoResponse.Entity;
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
                    var entity = Repository.Congregations.GetSingle(ID).Entity;
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
                return Json(new JsonResponse { Succeeded = false, Message = string.Format("CongregationID {0} was not regognized.", id) });
            }
        }

        #endregion

        #region List Helpers

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult GetMissionCentersAjax()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var item in Repository.MissionCenters.All().ToList())
            {
                dict.Add(item.MissionCenterID.ToString(), item.Name);
            }
            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Private Helpers

        private IEnumerable<object> ConvertCongregations(IEnumerable<Congregation> congregations)
        {
            return congregations.Select(a =>
            {
                return ConvertCongregation(a);
            });
        }

        private object ConvertCongregation(Congregation congregation)
        {
            return new
            {
                // generic and required by reusable JavaScript
                ID = congregation.CongregationID,
                TabTitleString = congregation.Name,
                DateModified = congregation.DateModified.HasValue ? CmsContext.DateConverter.Convert(congregation.DateModified.Value).FromUtc().ForCmsUser().ToString("g") : "Never",

                // specific to this object
                Name = congregation.Name,
                City = congregation.City,
                State = congregation.State,
                Country = congregation.Country,
                MissionCenter = congregation.MissionCenter != null ? congregation.MissionCenter.Name : "",
            };
        }

        #endregion

    }
}