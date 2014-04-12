using System;
using System.Collections.Generic;
using System.Linq;
using Clickfarm.AppFramework.Extensions;
using KCActorsTheatre.Blogs;
using System.Globalization;
using KCActorsTheatre.Tags;

namespace KCActorsTheatre.Services.Mapping
{
	public class JsonMapper : IJsonMapper
    {

        #region Entities

        public object Post(Post post)
		{
			return new
			{
				postID = post.PostID,
				title = post.Title,
                teaser = post.Teaser,
                date = post.DateToPost.Value.ToString(@"dddd, MMMM dd", CultureInfo.CurrentCulture),
                author = post.Author,
                url = string.Format("{0}#/{1}", post.BlogType.GetDescription().ToLower().Replace(" ", "-"), post.PostID),
                body = post.Body,
                postCategories = PostCategories(post.Categories),
                tags = Tags(post.Tags),
                postURL = string.Format("{0}/{1}", post.PostID, post.Title.ToLower().Replace(" ", "-")),
			};
		}

        public object Posts(IEnumerable<Post> posts)
        {
            return GetObjectArray<Post>(posts, (post) =>
            {
                return Post(post);
            });
        }

        public object PostCategory(PostCategory postCategory)
        {
            return GetObject(postCategory, () =>
            {
                return new
                {
                    id = postCategory.PostCategoryID,
                    name = postCategory.Name,
                };
            });
        }

        public object PostCategories(IEnumerable<PostCategory> postCategories)
        {
            return GetObjectArray<PostCategory>(postCategories, (postCategory) =>
            {
                return PostCategory(postCategory);
            });
        }

        public object Tag(Tag tag)
        {
            return GetObject(tag, () =>
            {
                return new
                {
                    tagID = tag.TagID,
                    name = tag.Name,
                };
            });
        }

        public object Tags(IEnumerable<Tag> tags)
        {
            return GetObjectArray<Tag>(tags, (tag) =>
            {
                return Tag(tag);
            });
        }
        #endregion

        #region Messages

        public object Failed(string message, bool sessionExpired = false)
		{
			return new
			{
				succeeded = false,
				message = message,
				sessionExpired = sessionExpired
			};
		}
		public object Succeeded(string message)
		{
			return new
			{
				succeeded = true,
				message = message
			};
		}

        #endregion

        #region Private Helpers

        private object GetObject(object entity, Func<object> createObject)
		{
			if (entity == null)
			{
				return null;
			}
			return createObject();
		}

		private object GetObjectArray<T>(IEnumerable<T> entities, Func<T, object> createObject) where T : class
		{
			if (entities == null)
			{
				return new object[] { };
			}
			return entities.ToList().ConvertAll<object>(entity =>
			{
				return createObject(entity);
			}).ToArray();
        }

        #endregion

    }
}