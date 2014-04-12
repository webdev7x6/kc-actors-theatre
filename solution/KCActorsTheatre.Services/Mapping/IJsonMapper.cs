using System;
using System.Collections.Generic;
using KCActorsTheatre.Blogs;

namespace KCActorsTheatre.Services.Mapping
{
	public interface IJsonMapper
	{
        object PostCategory(PostCategory postCategory);
        object PostCategories(IEnumerable<PostCategory> postCategories);
        object Post(Post post);
        object Posts(IEnumerable<Post> posts);
		object Failed(string message, bool sessionExpired = false);
		object Succeeded(string message);
	}
}