using Clickfarm.AppFramework.Responses;
using Clickfarm.AppFramework.Extensions;
using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using Clickfarm.Cms.Core;
using KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Organizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Organizations;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.Controllers
{
    public class OrganizationsController : KCActorsTheatreAdminControllerBase
    {
        public OrganizationsController(ICmsContext context) : base(context) { }

        #region Views

        public ActionResult Affiliates()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            vm.DescriptionSingular = "Affiliate";
            vm.DescriptionPlural = "Affiliates";
            vm.OrganizationType = OrganizationType.Affiliate;
            return View("Index", vm);
        }

        public ActionResult Associations()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            vm.DescriptionSingular = "Association";
            vm.DescriptionPlural = "Associations";
            vm.OrganizationType = OrganizationType.Association;
            return View("Index", vm);
        }

        public ActionResult Ministries()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            vm.DescriptionSingular = "Ministry";
            vm.DescriptionPlural = "Ministries";
            vm.OrganizationType = OrganizationType.Ministry;
            return View("Index", vm);
        }

        public ActionResult Services()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            vm.DescriptionSingular = "Service";
            vm.DescriptionPlural = "Services";
            vm.OrganizationType = OrganizationType.Service;
            return View("Index", vm);
        }

        public ActionResult WorldChurchTeams()
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            vm.DescriptionSingular = "World Church Team";
            vm.DescriptionPlural = "World Church Teams";
            vm.OrganizationType = OrganizationType.WorldChurchTeam;
            return View("Index", vm);
        }

        #endregion

        #region Organizations

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult All(OrganizationType organizationType)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Organizations.GetAllByType(organizationType);
            if (repoResponse.Succeeded)
            {
                var items = ConvertOrganizations(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} organizations(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Find(string term, OrganizationType organizationType)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Organizations.FindForDisplay(term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries), organizationType);
            if (repoResponse.Succeeded)
            {
                var items = ConvertOrganizations(repoResponse.Entity);
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
        public JsonResult Create(Organization organization)
        {
            // set some defaults
            organization.DateCreated = DateTime.UtcNow;
            organization.IsPublished = true;

            if (ModelState.IsValid)
            {
                RepositoryResponse<Organization> repoResponse = Repository.Organizations.New(organization);
                if (repoResponse.Succeeded)
                {
                    var jsonResponse = new JsonResponse
                    {
                        Succeeded = true,
                        Message = "New organization created.",

                    };
                    jsonResponse.Properties.Add("Item", ConvertOrganization(repoResponse.Entity));
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
            var repoResponse = Repository.Organizations.GetSingle(id);
            if (repoResponse.Succeeded)
            {
                vm.Organization = repoResponse.Entity;
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
                    var entity = Repository.Organizations.GetSingle(ID).Entity;
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
                return Json(new JsonResponse { Succeeded = false, Message = string.Format("OrganizationID {0} was not regognized.", id) });
            }
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Delete(int id)
        {
            RepositoryResponse repoResponse = Repository.Organizations.Delete(id);
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("Organization with ID {0} deleted.", id));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        #endregion

        #region Private Helpers

        private IEnumerable<object> ConvertOrganizations(IEnumerable<Organization> items)
        {
            return items.Select(a =>
            {
                return ConvertOrganization(a);
            });
        }

        private object ConvertOrganization(Organization item)
        {
            return new
            {
                // generic and required by reusable JavaScript
                ID = item.OrganizationID,
                TabTitleString = item.Name,
                DateModified = item.DateModified.HasValue ? CmsContext.DateConverter.Convert(item.DateModified.Value).FromUtc().ForCmsUser().ToString("g") : "Never",

                // specific to this object
                Name = item.Name,
                IsPublished = item.IsPublished ? "Yes" : "No",
            };
        }

        #endregion

    }
}
