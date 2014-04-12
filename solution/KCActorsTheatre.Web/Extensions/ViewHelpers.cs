using KCActorsTheatre.Blogs;
using KCActorsTheatre.MissionStories;
using System;
using System.Web;
using System.Web.Mvc;

namespace KCActorsTheatre.Web.Extensions
{
    public static class ViewHelpers
    {

        public static string MissionStoryUrl(this UrlHelper url, MissionStory missionStory, HttpContextBase httpContext = null, bool includeHost = false)
        {
            var urlBase = "mission-story";

            string protocolAndHost = (httpContext != null && includeHost) ? string.Format("{0}://{1}", httpContext.Request.Url.Scheme, httpContext.Request.Url.Host) : string.Empty;

            return String.Format("{0}/{1}/{2}/{3}", protocolAndHost, urlBase, missionStory.MissionStoryID, HttpUtility.UrlEncode(missionStory.Title.Replace(" ", "-")));
        }

        public static string BlogPostUrl(this UrlHelper url, Post post, HttpContextBase httpContext = null, bool includeHost = false)
        {
            var urlBase = string.Empty;

            switch (post.BlogType)
            {
                case BlogType.Evangelist:
                    urlBase = "evangelist";
                    break;
                case BlogType.DailyBread:
                    urlBase = "daily-bread";
                    break;
            }

            string protocolAndHost = (httpContext != null && includeHost) ? string.Format("{0}://{1}", httpContext.Request.Url.Scheme, httpContext.Request.Url.Host) : string.Empty;

            return String.Format("{0}/{1}/{2}/{3}", protocolAndHost, urlBase, post.PostID, HttpUtility.UrlEncode(post.Title.Replace(" ", "-")));
        }

    }
}