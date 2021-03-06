﻿using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using Clickfarm.Cms.Admin.Mvc.Areas.Admin;
using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Contract;
using KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Show;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.Controllers
{
    [RequiresCmsUserAuthorization(Constants.CmsRole_CalendarManager)]
    public class ShowController : KCActorsTheatreAdminControllerBase
    {
        public ShowController(ICmsContext context) : base(context) { }

        public ViewResult Index()
        {
            var model = new ShowViewModel();
            model.Init(CmsContext);
            return View(model);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult CreateShowAjax(ShowInfo show)
        {
            JsonResponse jsonResponse = new JsonResponse();

            show.DateCreated = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                try
                {
                    var repoResponse = Repository.Shows.New(show);

                    if (repoResponse.Succeeded && repoResponse.Entity != null)
                    {
                        jsonResponse.Properties.Add("Item", ConvertShow(repoResponse.Entity));
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
        public ViewResult EditShowAjax(int id)
        {
            var vm = new EditShowViewModel
            {
                DateConverter = CmsContext.DateConverter,
                ContentProperties_Body_Html = new HtmlContentProperties(),
                ContentProperties_Body_ImageFile = new FileContentProperties
                {
                    RootFolder = "/Common/Cms/Images",
                    DefaultSubfolder = "ShowImages",
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
                    DefaultSubfolder = "ShowImages",
                    MediaTypes = new string[] { "image/" }
                }
            };
            var repoResponse = Repository.Shows.GetSingle(id);
            if (repoResponse.Succeeded)
            {
                vm.Show = repoResponse.Entity;
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult EditShowAjax(string id, string property, string newValue)
        {
            int ID = 0;
            if (int.TryParse(id, out ID))
            {
                try
                {
                    var entity = Repository.Shows.GetSingle(ID).Entity;
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
        public JsonResult DeleteShowAjax(int id)
        {
            RepositoryResponse repoResponse = Repository.Shows.Delete(id);
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
        public JsonResult FindShowsAjax(string term)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Shows.FindForDisplay(term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries));
            if (repoResponse.Succeeded)
            {
                var items = ConvertShows(repoResponse.Entity);
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
        public JsonResult AllShowsAjax()
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Shows.GetAll();
            if (repoResponse.Succeeded)
            {
                var items = ConvertShows(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} items(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        #region People

        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        //public JsonResult FindPeople(string term)
        //{
        //    var termsSplit = term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
        //    var people = Repository.People.Find(termsSplit);
        //    return Json(People(people.Entity));
        //}

        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        //public JsonResult AllPeople()
        //{
        //    var people = Repository.People.All();
        //    return Json(People(people));
        //}

        //public JsonResult AddPerson(int id, int personID)
        //{
        //    var repoResp = Repository.Shows.AddPerson(id, personID);
        //    return Json(new JsonResponse
        //    {
        //        Succeeded = repoResp.Succeeded,
        //        Message = repoResp.Message
        //    });
        //}

        //public JsonResult RemovePerson(int id, int personID)
        //{
        //    var repoResp = Repository.Shows.RemovePerson(id, personID);
        //    return Json(new JsonResponse
        //    {
        //        Succeeded = repoResp.Succeeded,
        //        Message = repoResp.Message
        //    });
        //}

        //private JsonResponse People(IEnumerable<Person> people)
        //{
        //    var resp = new JsonResponse();
        //    resp.Properties.Add("People", people);
        //    resp.Succeed(string.Format("{0} people found.", people.Count()));
        //    return resp;
        //}

        #endregion

        #region Images

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult AddImage(ShowImage showImage)
        {
            // set defaults
            showImage.DateCreated = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                RepositoryResponse<ShowImage> repoResponse = Repository.Images.New(showImage);
                if (repoResponse.Succeeded)
                {
                    var jsonResponse = new JsonResponse
                    {
                        Succeeded = true,
                        Message = "New image added.",

                    };
                    jsonResponse.Properties.Add("ShowImage", repoResponse.Entity);
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
        public JsonResult RemoveImage(int id)
        {
            RepositoryResponse repoResponse = Repository.Images.Delete(id);
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("image with ID {0} removed.", id));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult EditImageInPlace(string imageID, string property, string newValue)
        {
            int ID = 0;
            if (int.TryParse(imageID, out ID))
            {
                try
                {
                    var entity = Repository.Images.Single(p => p.ShowImageID == ID);
                    EditInPlaceJsonResponse response = EditProperty(editID => entity, imageID, property, newValue, null, null, null);
                    return Json(response);
                }
                catch (Exception ex)
                {
                    return Json(new JsonResponse { Succeeded = false, Message = string.Format("An exception occurred: {0}", ex.Message) });
                }
            }
            else
            {
                return Json(new JsonResponse { Succeeded = false, Message = string.Format("image {0} was not regognized.", imageID) });
            }
        }


        [HttpPost]
        public JsonResult UpdateImageDisplayOrder(int[] imageIDs)
        {
            var response = new JsonResponse();
            var repoResponse = Repository.Images.UpdateImageDisplayOrder(imageIDs);
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

        #region Videos

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult AddVideo(ShowVideo showVideo)
        {
            // set defaults
            showVideo.DateCreated = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                RepositoryResponse<ShowVideo> repoResponse = Repository.Videos.New(showVideo);
                if (repoResponse.Succeeded)
                {
                    var jsonResponse = new JsonResponse
                    {
                        Succeeded = true,
                        Message = "New video added.",

                    };
                    jsonResponse.Properties.Add("ShowVideo", repoResponse.Entity);
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
        public JsonResult RemoveVideo(int id)
        {
            RepositoryResponse repoResponse = Repository.Videos.Delete(id);
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("video with ID {0} removed.", id));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [HttpPost]
        public JsonResult UpdateVideoDisplayOrder(int[] videoIDs)
        {
            var response = new JsonResponse();
            var repoResponse = Repository.Videos.UpdateVideoDisplayOrder(videoIDs);
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

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult GetSeasons()
        {
            var dict = Repository.Seasons.All()
                .ToList()
                .ToDictionary(item => item.SeasonID.ToString(), item => item.Title);

            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<object> ConvertShows(IEnumerable<ShowInfo> items)
        {
            return items.Select(ConvertShow);
        }

        private object ConvertShow(ShowInfo item)
        {
            return new
            {
                // generic and required by reusable JavaScript
                ID = item.ShowID,
                TabTitleString = item.Title,
                DateCreated = CmsContext.DateConverter.Convert(item.DateCreated).FromUtc().ForCmsUser().ToString("g"),

                // specific to this object
                Title = item.Title,
                Body = item.Body,
                PreviewImageUrl = item.PreviewImageUrl,
                StartDate = item.StartDate.HasValue ? CmsContext.DateConverter.Convert(item.StartDate.Value).FromUtc().ForCmsUser().ToShortDateString() : string.Empty,
                EndDate = item.EndDate.HasValue ? CmsContext.DateConverter.Convert(item.EndDate.Value).FromUtc().ForCmsUser().ToShortDateString() : string.Empty,
            };
        }
    }
}