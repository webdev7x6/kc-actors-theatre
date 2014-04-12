using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Data;
using KCActorsTheatre.Staff;
using KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.StaffMembers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.Controllers
{
    public class StaffMembersController : KCActorsTheatreAdminControllerBase
    {
        public StaffMembersController(ICmsContext context) : base(context)
        {
        }

        #region Views

        public ActionResult Connect(int? id = null)
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            InitializeIndexViewModel(id, vm);

            vm.DescriptionSingular = "Connect Widget Contact";
            vm.DescriptionPlural = "Connect Widget Contacts";
            vm.StaffType = StaffType.ConnectWidgetContact;
            vm.TypeofStaffMember = typeof(ConnectWidgetContact);
            return View("Index", vm);
        }

        public ActionResult Staff(int? id = null)
        {
            var vm = new IndexViewModel();
            vm.Init(CmsContext);
            InitializeIndexViewModel(id, vm);

            vm.DescriptionSingular = "Staff Member";
            vm.DescriptionPlural = "Staff Members";
            vm.StaffType = StaffType.StaffMember;
            vm.TypeofStaffMember = typeof(RegularStaffMember);

            return View("Index", vm);
        }

        #endregion

        #region Staff Members

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Find(string term)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.StaffMembers.FindForDisplay(term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries));
            if (repoResponse.Succeeded)
            {
                var items = ConvertStaffMembers(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} staff member(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }
        
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult FindByType(string term, StaffType staffType)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.StaffMembers.FindByTypeForDisplay(term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries), staffType);
            if (repoResponse.Succeeded)
            {
                var items = ConvertStaffMembers(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} staff member(s) found.", items.Count()));
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
            var repoResponse = Repository.StaffMembers.GetAll();
            if (repoResponse.Succeeded)
            {
                var items = ConvertStaffMembers(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} staff members(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult AllByType(StaffType staffType)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.StaffMembers.GetAllByType(staffType);
            if (repoResponse.Succeeded)
            {
                var items = ConvertStaffMembers(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} staff members(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult Create(StaffMember staffMember, string TypeofStaffMember)
        {
            StaffMember newStaffMember = Activator.CreateInstance(Type.GetType(TypeofStaffMember)) as StaffMember;

            // set defaults
            newStaffMember.DateCreated = DateTime.UtcNow;
            newStaffMember.SortName = string.Format("{0}, {1}", staffMember.LastName, staffMember.FirstName);

            // copy from modelstate object
            newStaffMember.FirstName = staffMember.FirstName;
            newStaffMember.LastName = staffMember.LastName;

            if (ModelState.IsValid)
            {
                RepositoryResponse<StaffMember> repoResponse = Repository.StaffMembers.New(newStaffMember);
                if (repoResponse.Succeeded)
                {
                    var jsonResponse = new JsonResponse
                    {
                        Succeeded = true,
                        Message = "New staff member created.",

                    };
                    jsonResponse.Properties.Add("Item", ConvertStaffMember(repoResponse.Entity));
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
            RepositoryResponse repoResponse = Repository.StaffMembers.Delete(id);
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("Staff Member with ID {0} deleted.", id));
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
                    RootFolder = "/common/cms/images/staff",
                    DefaultSubfolder = "",
                    MediaTypes = new string[] { "image/" }
                },
            };
            var repoResponse = Repository.StaffMembers.GetSingle(id);
            if (repoResponse.Succeeded)
            {
                vm.StaffMember = repoResponse.Entity;
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
                    var entity = Repository.StaffMembers.GetSingle(ID).Entity;
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
                return Json(new JsonResponse { Succeeded = false, Message = string.Format("StaffMemberID {0} was not regognized.", id) });
            }
        }

        #endregion

        #region Roles

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult CreateRole(RoleDefinition roleDefinition)
        {
            // set defaults
            roleDefinition.DateCreated = DateTime.UtcNow;

            // remove id's not associated with role
            switch (roleDefinition.Role)
            {
                case Role.MissionField:
                    roleDefinition.MissionCenterID = null;
                    break;
                case Role.MissionCenter:
                    roleDefinition.MissionFieldID = null;
                    break;
            }


            if (ModelState.IsValid)
            {
                RepositoryResponse<RoleDefinition> repoResponse = Repository.StaffMembers.NewRoleDefinition(roleDefinition);
                if (repoResponse.Succeeded)
                {
                    var jsonResponse = new JsonResponse
                    {
                        Succeeded = true,
                        Message = "New role definition created.",

                    };

                    // get the role definition with all the includes
                    var returnRoleDefinition = Repository.StaffMembers
                        .GetSingle(repoResponse.Entity.StaffMemberID).Entity.RoleDefinitions
                            .FirstOrDefault(p => p.RoleDefinitionID == repoResponse.Entity.RoleDefinitionID)
                            ;

                    // get the role string
                    var roleType = EnumExtensions.GetDescription(roleDefinition.Role);
                    var locationName = string.Empty;

                    // get the role type location name
                    switch (roleDefinition.Role)
                    {
                        case Role.MissionField:
                            locationName = returnRoleDefinition.MissionField.Name;
                            break;
                        case Role.MissionCenter:
                            locationName = returnRoleDefinition.MissionCenter.Name;
                            break;
                    }

                    var returnData = new
                    {
                        Name = locationName,
                        Type = roleType,
                        Title = returnRoleDefinition.Title,
                        RoleDefinitionID = returnRoleDefinition.RoleDefinitionID,
                        StaffMemberID = returnRoleDefinition.StaffMemberID,
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
        public JsonResult DeleteRole(int memberID, int roleID)
        {
            RepositoryResponse repoResponse = Repository.StaffMembers.DeleteRole(memberID, roleID);
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

        #region List Helpers


        #endregion

        #region Widget Tagging

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult GetWidgetStaff(int contentID)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.StaffMembers.GetWidgetStaff(contentID);
            if (repoResponse.Succeeded)
            {
                var items = ConvertStaffMembers(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(repoResponse.Message);
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        public JsonResult AddWidgetStaff(int contentID, int staffMemberID)
        {
            var jsonResponse = new JsonResponse();
            var repoResponse = Repository.StaffMembers.AddWidgetStaff(contentID, staffMemberID);
            if (repoResponse.Succeeded)
            {
                var item = ConvertStaffMember(repoResponse.Entity);
                jsonResponse.Properties.Add("Item", item);
                jsonResponse.Succeed(repoResponse.Message);
            }
            else
            {
                jsonResponse.Fail(repoResponse.Message);
            }
            return Json(jsonResponse);
        }

        public JsonResult RemoveWidgetStaff(int contentID, int staffMemberID)
        {
            var jsonResponse = new JsonResponse();
            var repoResponse = Repository.StaffMembers.RemoveWidgetStaff(contentID, staffMemberID);
            if (repoResponse.Succeeded)
            {
                var item = ConvertStaffMember(repoResponse.Entity);
                jsonResponse.Properties.Add("Item", item);
                jsonResponse.Succeed(repoResponse.Message);
            }
            else
            {
                jsonResponse.Fail(repoResponse.Message);
            }
            return Json(jsonResponse);
        }

        public JsonResult UpdateStaffMembersDisplayOrder(int contentID, int[] staffMemberIDs)
        {
            var response = new JsonResponse();

            var repoResponse = Repository.StaffMembers.UpdateStaffMembersDisplayOrder(contentID, staffMemberIDs);
            if (repoResponse.Succeeded)
                response.Succeed(repoResponse.Message);
            else
                response.Fail(repoResponse.Message);

            return Json(response);
        }

        #endregion

        #region Private Helpers

        private void InitializeIndexViewModel(int? id, IndexViewModel vm)
        {
            vm.RoleTitles = Repository.StaffMembers.GetStaffMemberTitles().Entity;

            if (id.HasValue)
            {
                var repoResponse = Repository.StaffMembers.GetSingle(id.Value);
                if (repoResponse.Succeeded)
                {
                    vm.EditStaffMember = repoResponse.Entity;
                }
            }

            vm.MissionCenterList = Repository.MissionCenters.GetAll().Entity.ToList().ConvertAll<SelectListItem>(c =>
            {
                SelectListItem item = new SelectListItem
                {
                    Text = c.Name,
                    Value = c.MissionCenterID.ToString(),
                };
                return item;
            });

            vm.MissionFieldList = Repository.MissionFields.GetAll().Entity.ToList().ConvertAll<SelectListItem>(c =>
            {
                SelectListItem item = new SelectListItem
                {
                    Text = c.Name,
                    Value = c.MissionFieldID.ToString(),
                };
                return item;
            });
        }

        private IEnumerable<object> ConvertStaffMembers(IEnumerable<StaffMember> items)
        {
            return items.Select(a =>
            {
                return ConvertStaffMember(a);
            });
        }

        private object ConvertStaffMember(StaffMember item)
        {
            return new
            {
                // generic and required by reusable JavaScript
                ID = item.StaffMemberID,
                TabTitleString = item.SortName,
                DateModified = item.DateModified.HasValue ? CmsContext.DateConverter.Convert(item.DateModified.Value).FromUtc().ForCmsUser().ToString("g") : "Never",

                // specific to this object
                Name = string.Format("{0}, {1}", item.LastName, item.FirstName),
                Phone = item.Phone,
                Email = item.Email,
            };
        }

        #endregion

    }
}