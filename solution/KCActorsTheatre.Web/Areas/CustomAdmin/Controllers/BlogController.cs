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
using KCActorsTheatre.Blog;
using KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Blog;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.Controllers
{
    [RequiresCmsUserAuthorization(Constants.CmsRole_BlogManager)]
    public class BlogController : KCActorsTheatreAdminControllerBase
    {
        public BlogController(ICmsContext context) : base(context) { }

        public ViewResult Posts()
        {
            var model = new PostsViewModel();
            model.Init(CmsContext);
            return View(model);
        }

        public ViewResult Comments()
        {
            var model = new CommentsViewModel();
            model.Init(CmsContext);
            return View(model);
        }

        public ViewResult Authors()
        {
            var model = new AuthorsViewModel();
            model.Init(CmsContext);
            return View(model);
        }

        #region Create

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult CreatePostAjax(Post post)
        {
            post.PublishStatus = PublishStatus.Draft;
            post.DateCreated = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                RepositoryResponse<Post> repoResponse = Repository.Posts.NewPost(post);
                if (repoResponse.Succeeded)
                {
                    var jsonResponse = new JsonResponse
                    {
                        Succeeded = true,
                        Message = "New blog post created.",

                    };
                    jsonResponse.Properties.Add("Item", ConvertPost(repoResponse.Entity));
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
                    Message = string.Join(" ", ModelState.ErrorMessages())
                });
            }
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult CreateAuthorAjax(Author author)
        {
            author.DateCreated = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                RepositoryResponse<Author> repoResponse = Repository.Authors.New(author);
                if (repoResponse.Succeeded)
                {
                    var jsonResponse = new JsonResponse
                    {
                        Succeeded = true,
                        Message = "New author created.",

                    };
                    jsonResponse.Properties.Add("Item", ConvertAuthor(repoResponse.Entity));
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
                    Message = string.Join(" ", ModelState.ErrorMessages())
                });
            }
        }

        #endregion

        #region Edit

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ViewResult EditPostAjax(int id)
        {
            EditPostViewModel vm = new EditPostViewModel
            {
                DateConverter = CmsContext.DateConverter,
                ContentProperties_Body_Html = new HtmlContentProperties(),
                ContentProperties_Image = new ImageContentProperties
                {
                    ExactWidth = 870,
                    ExactHeight = 400,
                },
                ContentProperties_ImageFile = new FileContentProperties
                {
                    RootFolder = "/common/cms/images/blog",
                    DefaultSubfolder = "post",
                    MediaTypes = new string[] { "image/" }
                }
            };
            var repoResponse = Repository.Posts.GetSingle(id);
            if (repoResponse.Succeeded)
            {
                vm.Post = repoResponse.Entity;
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult EditPostAjax(string id, string property, string newValue)
        {
            int ID = 0;
            if (int.TryParse(id, out ID))
            {
                try
                {
                    Func<string, string, object> convertValue = null;
                    Action<Post, string, string> setValue = null;
                    Action<Post, ModelStateDictionary, string, string> validateEntity = null;
                    if (property == "PublishDate" || property == "UnpublishDate")
                    {
                        convertValue = (prop, newVal) =>
                        {
                            if (string.IsNullOrWhiteSpace(newVal))
                            {
                                return null;
                            }
                            return CmsContext.DateConverter.Convert(DateTime.Parse(newVal)).ToUtc().ForCmsUser();
                        };
                        validateEntity = (post, modelState, prop, newVal) =>
                        {
                            if (
                                post.PublishDate.HasValue
                                && post.UnpublishDate.HasValue
                                && post.PublishDate.Value.Date >= post.UnpublishDate.Value.Date
                            )
                            {
                                modelState.AddModelError(prop, "Unpublish date must come after Publish date.");
                            }
                        };
                    }
                    var entity = Repository.Posts.GetSingle(ID).Entity;
                    EditInPlaceJsonResponse response = EditProperty(editID => entity, id, property, newValue, convertValue, setValue, validateEntity);
                    return Json(response);
                }
                catch (Exception ex)
                {
                    return Json(new JsonResponse { Succeeded = false, Message = string.Format("An exception occurred: {0}", ex.Message) });
                }
            }
            else
            {
                return Json(new JsonResponse { Succeeded = false, Message = string.Format("Blog Post ID {0} was not regognized.", id) });
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ViewResult EditAuthorAjax(int id)
        {
            EditAuthorViewModel vm = new EditAuthorViewModel
            {
                DateConverter = CmsContext.DateConverter,
                ContentProperties_Image = new ImageContentProperties
                {
                    ExactWidth = 200,
                    ExactHeight = 200
                },
                ContentProperties_ImageFile = new FileContentProperties
                {
                    RootFolder = "/common/cms/images/blog",
                    DefaultSubfolder = "author",
                    MediaTypes = new string[] { "image/" }
                }
            };
            var repoResponse = Repository.Authors.GetSingle(id);
            if (repoResponse.Succeeded)
            {
                vm.Author = repoResponse.Entity;
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult EditAuthorAjax(string id, string property, string newValue)
        {
            int ID = 0;
            if (int.TryParse(id, out ID))
            {
                try
                {
                    var entity = Repository.Authors.GetSingle(ID).Entity;
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
                return Json(new JsonResponse { Succeeded = false, Message = string.Format("Author ID {0} was not regognized.", id) });
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ViewResult EditCommentAjax(int id)
        {
            EditCommentViewModel vm = new EditCommentViewModel
            {
                DateConverter = CmsContext.DateConverter,
                ContentProperties_Body_Html = new HtmlContentProperties(),
                ContentProperties_Body_ImageFile = new FileContentProperties
                {
                    RootFolder = "/Common/Cms/Images",
                    DefaultSubfolder = "CommentImages",
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
                    DefaultSubfolder = "CommentImages",
                    MediaTypes = new string[] { "image/" }
                }
            };
            var repoResponse = Repository.Comments.GetSingle(id);
            if (repoResponse.Succeeded)
            {
                vm.Comment = repoResponse.Entity;
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult EditCommentAjax(string id, string property, string newValue)
        {
            int ID = 0;
            if (int.TryParse(id, out ID))
            {
                try
                {
                    var entity = Repository.Comments.GetSingle(ID).Entity;
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
                return Json(new JsonResponse { Succeeded = false, Message = string.Format("Author ID {0} was not regognized.", id) });
            }
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult ModifyComments(int[] idArray, string action)
        {
            RepositoryResponse repoResponse = Repository.Comments.Modify(idArray, action.ToLower());
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("Blog Comments {0}ed.", action.ToLower()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }


        #endregion

        #region Delete

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult DeletePostAjax(int id)
        {
            RepositoryResponse repoResponse = Repository.Posts.Delete(id);
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("Blog Post with ID {0} deleted.", id));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult DeleteAuthorAjax(int id)
        {
            RepositoryResponse repoResponse = Repository.Authors.Delete(id);
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("Author with ID {0} deleted.", id));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult DeleteCommentAjax(int id)
        {
            RepositoryResponse repoResponse = Repository.Comments.Delete(id);
            JsonResponse response = new JsonResponse();
            if (repoResponse.Succeeded)
            {
                response.Succeed(string.Format("Comment with ID {0} deleted.", id));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        #endregion

        #region Find

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult FindPostsAjax(string term)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Posts.FindPostsForAdmin(term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries));
            if (repoResponse.Succeeded)
            {
                var posts = ConvertPosts(repoResponse.Entity);
                response.Properties.Add("Items", posts);
                response.Succeed(string.Format("{0} blog post(s) found.", posts.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult FindAuthorsAjax(string term)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Authors.FindForDisplay(term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries));
            if (repoResponse.Succeeded)
            {
                var items = ConvertAuthors(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} author(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult FindCommentsAjax(string term)
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Comments.FindForDisplay(term.Split(new char[0], StringSplitOptions.RemoveEmptyEntries));
            if (repoResponse.Succeeded)
            {
                var items = ConvertComments(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} items(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        #endregion

        #region Get All

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult AllPostsAjax()
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Posts.GetAll();
            if (repoResponse.Succeeded)
            {
                var posts = ConvertPosts(repoResponse.Entity);
                response.Properties.Add("Items", posts);
                response.Succeed(string.Format("{0} blog posts(s) found.", posts.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult AllAuthorsAjax()
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Authors.GetAll();
            if (repoResponse.Succeeded)
            {
                var items = ConvertAuthors(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} author(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult AllCommentsAjax()
        {
            JsonResponse response = new JsonResponse();
            var repoResponse = Repository.Comments.GetAll();
            if (repoResponse.Succeeded)
            {
                var items = ConvertComments(repoResponse.Entity);
                response.Properties.Add("Items", items);
                response.Succeed(string.Format("{0} comment(s) found.", items.Count()));
            }
            else
            {
                response.Fail(repoResponse.Message);
            }
            return Json(response);
        }

        #endregion

        #region Get Lists

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult GetPostStatusesAjax()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (PublishStatus s in EnumExtensions.GetMembers<PublishStatus>())
            {
                dict.Add(s.ToString(), s.GetDescription());
            }
            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult GetAuthorsAjax()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var author in Repository.Authors.All().ToList())
            {
                dict.Add(author.AuthorID.ToString(), author.Name);
            }
            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Private Object Converters

        private IEnumerable<object> ConvertPosts(IEnumerable<Post> posts)
        {
            return posts.Select(a =>
            {
                return ConvertPost(a);
            });
        }

        private object ConvertPost(Post post)
        {
            return new
            {
                // generic and required by reusable JavaScript
                ID = post.PostID,
                TabTitleString = post.Title,
                DateCreated = CmsContext.DateConverter.Convert(post.DateCreated).FromUtc().ForCmsUser().ToString("g"),

                // specific to this object
                Title = post.Title,
                Body = post.Body,
                ImageUrl = post.ImageURL,
                PublishDate = post.PublishDate.HasValue ? CmsContext.DateConverter.Convert(post.PublishDate.Value).FromUtc().ForCmsUser().ToShortDateString() : string.Empty,
                UnpublishDate = post.UnpublishDate.HasValue ? CmsContext.DateConverter.Convert(post.UnpublishDate.Value).FromUtc().ForCmsUser().ToShortDateString() : string.Empty,
                PublishStatus = post.PublishStatus.ToString(),
                Author = new
                {
                    Name = post.Author != null ? post.Author.Name : string.Empty,
                },
                IsViewable = post.IsViewable ? "Yes" : "No"
            };
        }

        private IEnumerable<object> ConvertAuthors(IEnumerable<Author> authors)
        {
            return authors.Select(a =>
            {
                return ConvertAuthor(a);
            });
        }

        private object ConvertAuthor(Author author)
        {
            return new
            {
                // generic and required by reusable JavaScript
                ID = author.AuthorID,
                TabTitleString = author.Name,
                DateCreated = CmsContext.DateConverter.Convert(author.DateCreated).FromUtc().ForCmsUser().ToString("g"),

                // specific to this object
                Name = author.Name,
                Description = author.Description,
            };
        }

        private IEnumerable<object> ConvertComments(IEnumerable<Comment> items)
        {
            return items.Select(a =>
            {
                return ConvertComment(a);
            });
        }

        private object ConvertComment(Comment item)
        {
            return new
            {
                // generic and required by reusable JavaScript
                ID = item.CommentID,
                TabTitleString = item.Name,
                DateCreated = CmsContext.DateConverter.Convert(item.DateCreated).FromUtc().ForCmsUser().ToString("g"),

                // specific to this object
                Name = item.Name,
                Text = item.Text,
                ApprovalText = item.IsApproved ? "Yes" : "No",
                IsApproved = item.IsApproved,
                Post = new
                {
                    Title = item.Post.Title,
                },
            };
        }

        #endregion

    }
}