using Clickfarm.AppFramework.Responses;
using Clickfarm.AppFramework.Web;
using Clickfarm.Cms.Core;
using Clickfarm.AppFramework.Extensions;
using KCActorsTheatre.Blog;
using KCActorsTheatre.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Clickfarm.AppFramework.Mail;
using KCActorsTheatre.Library.AppTypes;

namespace KCActorsTheatre.Web.Controllers
{
    public class BlogController : KCActorsTheatreController
    {
        KCActorsTheatreApp app;
        HttpContextBase httpContext;
        public BlogController(ICmsContext context, HttpContextBase httpContext) : base(context, httpContext) 
        {
            this.app = (KCActorsTheatreApp)repository.Apps.Find(1);
            this.httpContext = httpContext;
        }

        public ActionResult Index()
        {
            var model = new BlogViewModel();
            InitializeViewModel(model);
            model.Posts = repository.Posts
                .GetPostedAndPublished(5, 0)
                .Entity;
            model.JsonPosts = ConvertPosts(model.Posts);

            return View(model);
        }

        public ActionResult Post(int id)
        {
            var model = new BlogPostViewModel();
            InitializeViewModel(model);

            try
            {
                var repoReponse = repository.Posts.GetSinglePostedAndPublished(id, DateTime.UtcNow);

                if (repoReponse.Succeeded && repoReponse.Entity != null)
                {
                    model.Post = repoReponse.Entity;
                    model.Comment.PostID = model.Post.PostID;

                    // get previous and next posts' id's
                    var previousPostResponse = repository.Posts.GetPreviousOrNext(model.Post.PublishDate.Value, "previous");
                    var nextPostResponse = repository.Posts.GetPreviousOrNext(model.Post.PublishDate.Value, "next");

                    if (previousPostResponse.Succeeded && previousPostResponse.Entity != null)
                        model.PreviousPostID = previousPostResponse.Entity.PostID;
                    if (nextPostResponse.Succeeded && nextPostResponse.Entity != null)
                        model.NextPostID = nextPostResponse.Entity.PostID;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View(model);
        }

        [AjaxOnly]
        public JsonResult NewComment(Comment comment)
        {
            JsonResponse jsonResponse = new JsonResponse();

            comment.DateCreated = DateTime.UtcNow;
            comment.IsApproved = false;

            if (ModelState.IsValid)
            {
                try
                {
                    var repoReponse = repository.Comments.New(comment);

                    if (repoReponse.Succeeded)
                    {
                        jsonResponse.Succeeded = true;

                        comment.Post = repository.Posts.Single(p => p.PostID == comment.PostID, null, enableTracking: false);

                        // succeeded, send email notification
                        var title = "New comment requires moderation on KCActorsTheatre.net";
                        StringBuilder sb = new StringBuilder("<html>");
                        sb.AppendFormat("<head><title>{0}</title></head><body>", title);
                        sb.AppendFormat("<p>{0}</p>", title);
                        sb.AppendFormat("<p>Blog Post: {0}</p>", comment.Post.Title);
                        sb.AppendFormat("<p>Name: {0}</p>", comment.Name);
                        sb.AppendFormat("<p>Comment: {0}</p>", comment.Text);
                        sb.Append("</body></html>");

                        MailUtil mail = new MailUtil();
                        MailProperties props = new MailProperties()
                        {
                            From = "noreply@KCActorsTheatre.net",
                            IsBodyHtml = true,
                            Body = sb.ToString(),
                            Subject = title
                        };

                        props.To.Add(app.BlogCommmentEmailNotifications);
                        props.HttpContext = httpContext;
                        mail.SendEmail(props);
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
                jsonResponse.Fail("Validation failed.");
            }


            return Json(jsonResponse);
        }

        [AjaxOnly]
        public JsonResult GetPosts(int howMany, int skip)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                var repoReponse = repository.Posts
                    .GetPostedAndPublished(howMany, skip)
                ;

                if (repoReponse.Succeeded && repoReponse.Entity != null)
                {
                    jsonResponse.Properties.Add("Items", ConvertPosts(repoReponse.Entity));
                    jsonResponse.Succeeded = true;
                }
            }
            catch (Exception ex)
            {
                jsonResponse.Succeeded = false;
                jsonResponse.Message = ex.GetInnermostException().Message;
            }

            return Json(jsonResponse);
        }

        [AjaxOnly]
        public JsonResult Search(string searchTerm)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                var repoReponse = repository.Posts.FindPostsForWebsite(searchTerm);

                if (repoReponse.Succeeded && repoReponse.Entity != null)
                {
                    jsonResponse.Properties.Add("Items", ConvertPosts(repoReponse.Entity));
                    jsonResponse.Succeeded = true;
                }
            }
            catch (Exception ex)
            {
                jsonResponse.Succeeded = false;
                jsonResponse.Message = ex.GetInnermostException().Message;
            }

            return Json(jsonResponse);
        }

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
                ID = post.PostID,
                Title = post.Title,
                //ImageUrl = post.ImageURL,
                PublishDate = post.PublishDate.Value.ToShortDateString(),
                Author = new
                {
                    Name = post.Author != null ? post.Author.Name : string.Empty,
                },
                Summary = post.Summary,
                CommentCount = post.Comments.Count,
            };
        }
    }
}