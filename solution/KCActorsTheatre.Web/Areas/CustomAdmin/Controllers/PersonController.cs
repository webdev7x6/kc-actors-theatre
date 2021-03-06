﻿using System;
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
using KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.PersonModel;
using KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Show;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.Controllers
{
    public class PersonController : KCActorsTheatreAdminControllerBase
    {
        public PersonController(ICmsContext context) : base(context) { }

        public ViewResult Index()
        {
            var model = new PersonViewModel();
            model.Init(CmsContext);
            model.ShowList = Repository.Shows.GetAll().Entity.OrderBy(p => p.Title).ToList().ConvertAll<SelectListItem>(c =>
            {
                SelectListItem item = new SelectListItem
                {
                    Text = c.Title,
                    Value = c.ShowID.ToString(),
                };
                return item;
            });
            return View(model);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult CreatePersonAjax(Person person)
        {
            JsonResponse jsonResponse = new JsonResponse();

            person.DateCreated = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                try
                {
                    var repoResponse = Repository.People.New(person);

                    if (repoResponse.Succeeded && repoResponse.Entity != null)
                    {
                        jsonResponse.Properties.Add("Item", ConvertPerson(repoResponse.Entity));
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
        public ViewResult EditPersonAjax(int id)
        {
            var vm = new EditPersonViewModel
            {
                DateConverter = CmsContext.DateConverter,
                ContentProperties_Body_Html = new HtmlContentProperties(),
                ContentProperties_Body_ImageFile = new FileContentProperties
                {
                    RootFolder = "/Common/Cms/Images",
                    DefaultSubfolder = "PersonImages",
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
                    DefaultSubfolder = "PersonImages",
                    MediaTypes = new string[] { "image/" }
                }
            };
            var repoResponse = Repository.People.GetSingle(id);
            if (repoResponse.Succeeded)
            {
                vm.Person = repoResponse.Entity;
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult EditPersonAjax(string id, string property, string newValue)
        {
            int ID = 0;
            if (int.TryParse(id, out ID))
            {
                try
                {
                    var entity = Repository.People.GetSingle(ID).Entity;
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
        public JsonResult DeletePersonAjax(int id)
        {
            RepositoryResponse repoResponse = Repository.People.Delete(id);
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
        public JsonResult FindPeopleAjax(string term)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.People.FindForDisplay(term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries));
            if (repoResponse.Succeeded)
            {
                var items = ConvertPeople(repoResponse.Entity);
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
        public JsonResult AllPeopleAjax()
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.People.GetAll();
            if (repoResponse.Succeeded)
            {
                var items = ConvertPeople(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} items(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        #region Roles

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult AddRole(RoleDefinition roleDefinition)
        {
            // set defaults
            roleDefinition.DateCreated = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                RepositoryResponse<RoleDefinition> repoResponse = Repository.People.AddRole(roleDefinition);
                if (repoResponse.Succeeded)
                {
                    var jsonResponse = new JsonResponse
                    {
                        Succeeded = true,
                        Message = "New role definition created.",

                    };

                    // get the role definition with all the includes
                    var returnRoleDefinition = Repository.People
                        .GetSingle(repoResponse.Entity.PersonID).Entity.RoleDefinitions
                            .FirstOrDefault(p => p.RoleDefinitionID == repoResponse.Entity.RoleDefinitionID)
                            ;

                    var returnData = new
                    {
                        Show = returnRoleDefinition.Show.Title,
                        Title = returnRoleDefinition.Title,
                        RoleDefinitionID = returnRoleDefinition.RoleDefinitionID,
                        PersonID = returnRoleDefinition.PersonID,
                    };

                    jsonResponse.Properties.Add("RoleDefinition", returnData);
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
        public JsonResult DeleteRole(int personID, int roleID)
        {
            RepositoryResponse repoResponse = Repository.People.DeleteRole(personID, roleID);
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("Role Definition with ID {0} deleted.", roleID));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        #endregion


        private IEnumerable<object> ConvertPeople(IEnumerable<Person> items)
        {
            return items.Select(ConvertPerson);
        }

        private object ConvertPerson(Person item)
        {
            return new
            {
                // generic and required by reusable JavaScript
                ID = item.PersonID,
                TabTitleString = item.Name,
                DateCreated = CmsContext.DateConverter.Convert(item.DateCreated).FromUtc().ForCmsUser().ToString("g"),

                // specific to this object
                Name = item.Name,
                Title = item.Title,
                Body = item.BioDetail,
                BioSummary = item.BioSummary,
                ImageUrl = item.ImageUrl
            };
        }
    }
}