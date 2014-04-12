using Clickfarm.AppFramework.Responses;
using KCActorsTheatre;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Clickfarm.AppFramework.Extensions;
using KCActorsTheatre.Resources;
using System.Data;
using KCActorsTheatre.Tags;
using KCActorsTheatre.Cms.ContentTypes;
using KCActorsTheatre.Blogs;

namespace KCActorsTheatre.Data.Repositories
{
    public class PostRepository : KCActorsTheatreRepositoryBase<Post>
    {
        private IKCActorsTheatreRepository _repository;
        private IKCActorsTheatreDbContext _context;
        public PostRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository)
            : base(context, repository)
        {
            _context = context;
            _repository = repository;
        }

        protected override DbSet<Post> DbSet
        {
            get { return dbContext.Posts; }
        }

        public RepositoryResponse Delete(int id)
        {
            return CatchError<RepositoryResponse>(() =>
            {
                var item = DbSet
                    .Single(p => p.PostID == id);

                var response = new RepositoryResponse();

                if (item != null)
                {
                    CmsDbContext.ChangeState<Post>(EntityState.Deleted, item);
                    CmsDbContext.Save();
                    response.Succeed(string.Format("Post with ID {0} deleted.", id));
                }
                else
                {
                    response.Fail(string.Format("Post with ID {0} not found.", id));
                }
                return response;
            });
        }

        public RepositoryResponse<Post> New(Post item)
        {
            return CatchError<RepositoryResponse<Post>>(() =>
            {
                DbSet.Add(item);
                CmsDbContext.Save();
                var response = new RepositoryResponse<Post>();
                response.Succeed("The new Post was created successfully.");
                response.Entity = item;
                return response;
            });
        }

        public RepositoryResponse<Post> GetSingle(int id)
        {
            return CatchError<RepositoryResponse<Post>>(() =>
            {
                var item = DbSet
                    .Include(p => p.Tags)
                    .Include(p => p.Categories)
                    .SingleOrDefault(p => p.PostID == id)
                    ;

                var response = new RepositoryResponse<Post>();
                if (item != null)
                {
                    response.Succeed(string.Format("Item with ID {0} found.", item.PostID));
                    response.Entity = item;
                }
                else
                {
                    response.Fail(string.Format("Item with ID {0} not found.", id));
                }
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Post>> GetAllByType(BlogType blogType)
        {
            //return CatchError<RepositoryResponse<IEnumerable<Post>>>(() =>
            //{
                var response = new RepositoryResponse<IEnumerable<Post>>();

                var items = DbSet
                    .Where(p => p.BlogType == blogType)
                    .AsNoTracking()
                    ;


                if (items != null)
                {
                    response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                    response.Entity = items.OrderBy(a => a.Title);
                }
                else
                {
                    response.Fail("No posts found");
                }
                return response;
            //});
        }

        public RepositoryResponse<IEnumerable<Post>> FindByTypeForDisplay(IEnumerable<string> terms, BlogType blogType)
        {
            return CatchError<RepositoryResponse<IEnumerable<Post>>>(() =>
            {
                var items = All(null, enableTracking: false)
                    .Where(p => p.BlogType == blogType);

                if (terms != null && terms.Count() > 0)
                {
                    items = items.Where(a =>
                        terms.Any(t =>
                            a.Title != null && a.Title.IndexOf(t, StringComparison.CurrentCultureIgnoreCase) >= 0
                        )
                    );
                }

                var response = new RepositoryResponse<IEnumerable<Post>>();
                response.Succeed(string.Format("{0} items(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.Title);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Post>> Find(IEnumerable<string> terms, BlogType blogType)
        {
            var resp = new RepositoryResponse<IEnumerable<Post>>();
            try
            {
                var tags = this.All();
                resp.Entity = tags
                    .Where(p => p.BlogType == blogType)
                    .Where(c => terms.Any(t => c.Title.ToLower().IndexOf(t.ToLower()) > -1))
                    .OrderBy(c => c.Title)
                    .ToList()
                ;
                resp.Succeed(string.Format("{0} post(s) found.", resp.Entity.Count()));
            }
            catch (Exception ex)
            {
                resp.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return resp;
        }

        public RepositoryResponse<Tag> AddTag(int id, int tagID)
        {
            var resp = new RepositoryResponse<Tag>();
            try
            {
                var tag = _context.Tags
                    .Single(p => p.TagID == tagID);

                var item = this.Single(p => p.PostID == id, new string[] { "Tags" }, true);

                if (tag != null && item != null)
                {
                    item.Tags.Add(tag);
                    _context.Save();
                    resp.Entity = tag;
                    resp.Succeed(string.Format("Tag {0} added to Post {1}.", tag.Name, item.Title));
                }
                else
                {
                    resp.Fail(string.Format("Post {0} not found.", id));
                }
            }
            catch (Exception ex)
            {
                resp.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return resp;
        }

        public RepositoryResponse RemoveTag(int id, int tagID)
        {
            var resp = new RepositoryResponse();
            try
            {
                var tag = _context.Tags
                    .Single(p => p.TagID == tagID);

                var item = this.Single(p => p.PostID == id, new string[] { "Tags" }, true);

                if (tag != null && item != null)
                {
                    item.Tags.Remove(tag);
                    _context.Save();
                    resp.Succeed(string.Format("Tag {0} removed from Post {1}.", tag.Name, item.Title));
                }
                else
                {
                    resp.Fail(string.Format("Post with id {0} not found.", id));
                }
            }
            catch (Exception ex)
            {
                resp.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return resp;
        }

        public RepositoryResponse<PostCategory> AddCategory(int postID, int categoryID)
        {
            var resp = new RepositoryResponse<PostCategory>();
            try
            {
                var category = _context.PostCategories
                    .Single(p => p.PostCategoryID == categoryID);

                var post = this.Single(p => p.PostID == postID, new string[] { "Categories" }, true);

                if (category != null && post != null)
                {
                    post.Categories.Add(category);
                    _context.Save();
                    resp.Entity = category;
                    resp.Succeed(string.Format("Category {0} added to Post {1}.", category.Name, post.Title));
                }
                else
                {
                    resp.Fail(string.Format("Post id {0} not found.", postID));
                }
            }
            catch (Exception ex)
            {
                resp.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return resp;
        }

        public RepositoryResponse RemoveCategory(int postID, int categoryID)
        {
            var resp = new RepositoryResponse();
            try
            {
                var category = _context.PostCategories
                    .Single(p => p.PostCategoryID == categoryID);

                var post = this.Single(p => p.PostID == postID, new string[] { "Categories" }, true);

                if (category != null && post != null)
                {
                    post.Categories.Remove(category);
                    _context.Save();
                    resp.Succeed(string.Format("Category {0} removed from Post {1}.", category.Name, post.Title));
                }
                else
                {
                    resp.Fail(string.Format("Post id {0} not found.", postID));
                }
            }
            catch (Exception ex)
            {
                resp.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return resp;
        }

        #region Website Methods

        /// <summary>
        /// Gets the post as well as the previous and next posts
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RepositoryResponse<PostsForPage> GetForDetailPage(int id)
        {
            return CatchError<RepositoryResponse<PostsForPage>>(() =>
            {
                var response = new RepositoryResponse<PostsForPage>();
                var postsForPage = new PostsForPage();

                IEnumerable<Post> posts = DbSet
                    .AsNoTracking()
                    .Include(p => p.Tags)
                    .Where(p => p.Status == PostStatus.Published)
                    .Where(p => p.DateToPost.HasValue && p.DateToPost.Value < DateTime.UtcNow)
                    .OrderByDescending(p => p.DateToPost.Value)
                    .ThenBy(p => p.Title)
                    ;

                postsForPage.Post = posts.FirstOrDefault(p => p.PostID == id);

                if (postsForPage.Post != null)
                {
                    postsForPage.NextPost = posts
                        .Where(p => p.BlogType == postsForPage.Post.BlogType)
                        .Reverse()
                        .SkipWhile(p => p.PostID != postsForPage.Post.PostID)
                        .Skip(1)
                        .FirstOrDefault()
                        ;

                    postsForPage.PreviousPost = posts
                        .Where(p => p.BlogType == postsForPage.Post.BlogType)
                        .SkipWhile(p => p.PostID != postsForPage.Post.PostID)
                        .Skip(1)
                        .FirstOrDefault()
                        ;

                    response.Succeed(string.Format("Successfully retrieved blog post: {0}", postsForPage.Post.Title));
                    response.Entity = postsForPage;
                }
                else
                {
                    response.Fail("Blog post not found.");
                }

                return response;
            });
        }

        public RepositoryResponse<Post> GetPostForSite(int postID)
        {
            return CatchError<RepositoryResponse<Post>>(() =>
            {
                var response = new RepositoryResponse<Post>();

                var post = DbSet
                    .AsNoTracking()
                    .Where(p => p.Status == PostStatus.Published)
                    .Where(p => p.DateToPost.HasValue && p.DateToPost.Value < DateTime.UtcNow)
                    .FirstOrDefault(p => p.PostID == postID)
                    ;

                if (post != null)
                {
                    response.Succeed(string.Format("post with id {0} found", post.PostID));
                    response.Entity = post;
                }
                else
                {
                    response.Fail(string.Format("post with id {0} not found", postID));
                }

                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Post>> GetPostsForSite(BlogType blogType, int categoryID, int? month = null, int? year = null)
        {
            return CatchError<RepositoryResponse<IEnumerable<Post>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<Post>>();

                var posts = DbSet
                    .Include(p => p.Categories)
                    .Include(p => p.Tags)
                    .Where(p => p.Status == PostStatus.Published)
                    .Where(p => p.BlogType == blogType)
                    .Where(p => p.DateToPost.HasValue && p.DateToPost.Value < DateTime.UtcNow)
                    .OrderByDescending(p => p.DateToPost.Value)
                    .ThenBy(p => p.Title)
                    .AsNoTracking();

                if (categoryID > 0)
                {
                    posts = posts
                        .Where(p => p.Categories
                            .Any(c => c.PostCategoryID == categoryID))
                            ;
                }
                else if (month != null && year != null)
                {
                    posts = posts
                        .Where(p => p.DateToPost.Value.Year == year.Value)
                        .Where(p => p.DateToPost.Value.Month == month.Value)
                        ;
                }

                if (posts != null)
                {
                    response.Succeed(string.Format("{0} post(s) found.", posts.Count()));
                    response.Entity = posts;
                }
                else
                {
                    response.Fail("no posts found");
                }

                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Post>> GetPostsByCategoryForSite(BlogType blogType, string category, int skip, int take)
        {
            return CatchError<RepositoryResponse<IEnumerable<Post>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<Post>>();

                var posts = DbSet
                    .Include(p => p.Categories)
                    .Include(p => p.Tags)
                    .Where(p => p.Status == PostStatus.Published)
                    .Where(p => p.BlogType == blogType)
                    .Where(p => p.DateToPost.HasValue && p.DateToPost.Value < DateTime.UtcNow)
                    .Where(p => p.Categories.Any(c => c.Name == category))
                    .OrderByDescending(p => p.DateToPost.Value)
                    .ThenBy(p => p.Title)
                    .Skip(skip)
                    .Take(take)
                    .AsNoTracking();

                if (posts != null)
                {
                    response.Succeed(string.Format("{0} post(s) found.", posts.Count()));
                    response.Entity = posts;
                }
                else
                {
                    response.Fail("no posts found");
                }

                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Post>> GetPostsByMonthForSite(BlogType blogType, int month, int year, int skip, int take)
        {
            return CatchError<RepositoryResponse<IEnumerable<Post>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<Post>>();

                var posts = DbSet
                    .Include(p => p.Categories)
                    .Include(p => p.Tags)
                    .Where(p => p.Status == PostStatus.Published)
                    .Where(p => p.BlogType == blogType)
                    .Where(p => p.DateToPost.HasValue && p.DateToPost.Value < DateTime.UtcNow)
                    .Where(p => p.DateToPost.Value.Year == year)
                    .Where(p => p.DateToPost.Value.Month == month)
                    .OrderByDescending(p => p.DateToPost.Value)
                    .ThenBy(p => p.Title)
                    .Skip(skip)
                    .Take(take)
                    .AsNoTracking();

                if (posts != null)
                {
                    response.Succeed(string.Format("{0} post(s) found.", posts.Count()));
                    response.Entity = posts;
                }
                else
                {
                    response.Fail("no posts found");
                }

                return response;
            });
        }

        public int GetPostCount(BlogType blogType)
        {
            var response = 0;

            var posts = DbSet
                .Where(p => p.Status == PostStatus.Published)
                .Where(p => p.BlogType == blogType)
                .Where(p => p.DateToPost.HasValue && p.DateToPost.Value < DateTime.UtcNow)
                .AsNoTracking();

            if (posts != null)
            {
                response = posts.Count();
            }

            return response;
        }



        #endregion


    }
}