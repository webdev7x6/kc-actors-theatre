using System.Collections.Generic;
using System.Linq;
using Clickfarm.AppFramework.Web;
using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;

namespace KCActorsTheatre.Blog
{
    public class PostDTO
    {
        public string Author { get; private set; }

        public string Title { get; private set; }

        public string Teaser { get; private set; }

        public string Body { get; private set; }

        public string ImageUrl { get; private set; }

        public string DateToPost { get; private set; }

        public string Category { get; private set; }

        public IEnumerable<string> Tags { get; private set; }

        public string Url { get; private set; }

        public string IndexUrl { get; private set; }

        public PostDTO(RequestContent cmsRequestContent, Post article, bool excludeBody)
        {
            //Author = article.Author;
            //Title = article.Title;
            //Teaser = article.Teaser;
            //if (!excludeBody)
            //{
            //    Body = article.Body;
            //}
            //ImageUrl = article.ImageUrl;
            //DateToPost = article.DateToPost.Value.ToString("d");
            //if (article.ArticleCategory != null)
            //{
            //    Category = article.ArticleCategory.Name;
            //}
            //Tags = article.ArticleTags.Select(t => t.Name);
            //Url = BuildNewsArticleLink(cmsRequestContent, article);
            //IndexUrl = MvcExtensions.GetCmsUrlPathByKey(null, cmsRequestContent, Constants.CmsKey_Url_NewsIndex);
        }
    }
}
