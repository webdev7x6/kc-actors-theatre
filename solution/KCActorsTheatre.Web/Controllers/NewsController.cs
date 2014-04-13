using Clickfarm.AppFramework.Responses;
using Clickfarm.AppFramework.Web;
using Clickfarm.Cms.Core;
using Clickfarm.AppFramework.Extensions;
using KCActorsTheatre.Contract;
using KCActorsTheatre.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCActorsTheatre.Web.Controllers
{
    public class NewsController : KCActorsTheatreController
    {
        public NewsController(ICmsContext context, HttpContextBase httpContext)
            : base(context, httpContext) 
        {
        }

        public ActionResult Index()
        {
            var model = new NewsViewModel();
            InitializeViewModel(model);

            model.NewsArticles = repository.NewsArticles.GetForWebsite(9, 0).Entity;

            return View(model);
        }

        public ActionResult Detail(int id)
        {
            var model = new NewsViewModel();
            InitializeViewModel(model);

            try
            {
                var repoReponse = repository.NewsArticles.GetSingle(id);
                if (repoReponse.Succeeded && repoReponse.Entity != null)
                    model.Article = repoReponse.Entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View("Article", model);
        }

        [AjaxOnly]
        public JsonResult GetEvents(int howMany, int skip)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                var repoReponse = repository.NewsArticles
                    .GetForWebsite(howMany, skip)
                ;

                if (repoReponse.Succeeded && repoReponse.Entity != null)
                {
                    jsonResponse.Properties.Add("Items", ConvertArticles(repoReponse.Entity));
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
                var repoReponse = repository.NewsArticles.FindForWebsite(searchTerm);

                if (repoReponse.Succeeded && repoReponse.Entity != null)
                {
                    jsonResponse.Properties.Add("Items", ConvertArticles(repoReponse.Entity));
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

        private IEnumerable<object> ConvertArticles(IEnumerable<Article> articles)
        {
            return articles.Select(a =>
            {
                return ConvertArticle(a);
            });
        }

        private object ConvertArticle(Article article)
        {
            var articleDate = article.ArticleDate.Value;
            return new
            {
                ID = article.ArticleID,
                StartDate = articleDate.ToString("dd"),
                StartMonth = articleDate.ToString("MMM"),
                Title = Truncate(article.Title, 18),
                Summary = Truncate(article.Summary, 70),
            };
        }

        private string Truncate(string value, int maxChars)
        {
            var returnValue = string.Empty;
            if (!string.IsNullOrWhiteSpace(value))
                returnValue = value.Length <= maxChars ? value : value.Substring(0, maxChars) + " ..";
            return returnValue;
        }
    }
}