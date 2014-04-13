using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using KCActorsTheatre.Blog;

namespace KCActorsTheatre.Data.Repositories
{
    public class PostRepository : KCActorsTheatreRepositoryBase<Post>
    {
        private IKCActorsTheatreRepository _repository;
        private IKCActorsTheatreDbContext _context;

        public PostRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository) : base(context, repository)
        {
            _context = context;
            _repository = repository;
        }

        protected override DbSet<Post> DbSet
        {
            get { return dbContext.Posts; }
        }

        public RepositoryResponse<Post> NewPost(Post post)
        {
            return CatchError<RepositoryResponse<Post>>(() =>
            {
                DbSet.Add(post);
                CmsDbContext.Save();
                var response = new RepositoryResponse<Post>();
                response.Succeed("The new blog post was created successfully.");
                response.Entity = post;
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Post>> FindPostsForAdmin(IEnumerable<string> terms)
        {
            return CatchError<RepositoryResponse<IEnumerable<Post>>>(() =>
            {
                var posts = All(new[] { "Author" }, enableTracking: false);
                if (terms != null && terms.Count() > 0)
                {
                    posts = posts.Where(a =>
                        terms.Any(t =>
                            a.Title != null && a.Title.IndexOf(t, StringComparison.CurrentCultureIgnoreCase) >= 0
                        )
                    );
                }
                var response = new RepositoryResponse<IEnumerable<Post>>();
                response.Succeed(string.Format("{0} blog post(s) found.", posts.Count()));
                response.Entity = posts.OrderBy(a => a.PublishDate).ThenBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Post>> FindPostsForWebsite(string searchTerm)
        {
            return CatchError<RepositoryResponse<IEnumerable<Post>>>(() =>
            {
                var posts = GetPostedAndPublished().Entity;
                if (searchTerm.Length > 0)
                {
                    posts = posts.Where(p => p.Title.Contains(searchTerm));
                }
                var response = new RepositoryResponse<IEnumerable<Post>>();
                response.Succeed(string.Format("{0} blog post(s) found.", posts.Count()));
                response.Entity = posts.OrderBy(a => a.PublishDate).ThenBy(a => a.DateCreated);
                return response;
            });
        }


        public RepositoryResponse<IEnumerable<Post>> GetAll()
        {
            return CatchError<RepositoryResponse<IEnumerable<Post>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<Post>>();
                var posts = All(new [] { "Author"}, enableTracking: false);
                response.Succeed(string.Format("{0} blog posts(s) found.", posts.Count()));
                response.Entity = posts.OrderBy(a => a.PublishDate).ThenBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Post>> GetAllPostedAndPublished()
        {
            return CatchError<RepositoryResponse<IEnumerable<Post>>>(() =>
            {
                DateTime now = DateTime.UtcNow.AbsoluteEnd();
                IEnumerable<Post> posts = ProcessIncludesAndTracking(
                    DbSet
                    .Include(p => p.Author)
                    .Include(p => p.Comments)
                    .Where(a =>
                        a.PublishDate.HasValue
                        && a.PublishDate.Value <= now
                        && (
                            !a.UnpublishDate.HasValue
                            || a.UnpublishDate.Value > now
                        )
                        && a.PublishStatus == PublishStatus.Published
                        && a.Author != null
                    ),
                    null,
                    false
                ).OrderByDescending(a => a.PublishDate).ThenByDescending(a => a.DateCreated);
                var response = new RepositoryResponse<IEnumerable<Post>>();
                response.Succeed(string.Format("{0} blog posts(s) found.", posts.Count()));
                response.Entity = posts;
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Post>> GetPostedAndPublished(int? howMany = null, int? skip = null)
        {
            return CatchError<RepositoryResponse<IEnumerable<Post>>>(() =>
            {
                var posts = GetAllPostedAndPublished().Entity;
                if (skip.HasValue)
                    posts = posts.Skip(skip.Value);
                if (howMany.HasValue)
                    posts = posts.Take(howMany.Value);
                var response = new RepositoryResponse<IEnumerable<Post>>();
                response.Succeed(string.Format("{0} blog posts(s) found.", posts.Count()));
                response.Entity = posts;
                return response;
            });
        }

        public RepositoryResponse<Post> GetSingle(int id)
        {
            return CatchError<RepositoryResponse<Post>>(() =>
            {
                var post = Single(a => a.PostID == id, new[] { "Author" }, enableTracking:true);
                var response = new RepositoryResponse<Post>();
                if (post != null)
                {
                    response.Succeed(string.Format("Blog Post with ID {0} found.", post.PostID));
                    response.Entity = post;
                }
                else
                {
                    response.Fail(string.Format("Blog Post with ID {0} not found.", id));
                }
                return response;
            });
        }

        public RepositoryResponse<Post> GetPreviousOrNext(DateTime publishDate, string direction)
        {
            return CatchError<RepositoryResponse<Post>>(() =>
            {
                var response = new RepositoryResponse<Post>();
                var posts = GetAllPostedAndPublished().Entity;
                Post post = null;

                switch (direction)
                {
                    case "next":
                        post = posts.OrderBy(p => p.PublishDate).FirstOrDefault(p => p.PublishDate > publishDate);
                        break;
                    case "previous":
                        post = posts.OrderByDescending(p => p.PublishDate).FirstOrDefault(p => p.PublishDate < publishDate);
                        break;
                }

                if (post != null)
                {
                    response.Succeed(string.Format("Blog Post with ID {0} found.", post.PostID));
                    response.Entity = post;
                }
                else
                {
                    response.Fail(string.Format("There is no {0} post to retrive.", direction));
                }
                return response;
            });
        }

        public RepositoryResponse<Post> GetSinglePostedAndPublished(int id, DateTime utcNow)
        {
            return CatchError<RepositoryResponse<Post>>(() =>
            {
                DateTime now = utcNow.AbsoluteEnd();
                var post = Single(
                    a =>
                        a.PostID == id
                        && a.PublishDate.HasValue
                        && a.PublishDate.Value <= now
                        && (
                            !a.UnpublishDate.HasValue
                            || a.UnpublishDate.Value > now
                        )
                        && a.PublishStatus == PublishStatus.Published
                    ,
                    new[] { "Author", "Author.Posts", "Comments" },
                    enableTracking:true
                );
                var response = new RepositoryResponse<Post>();
                if (post != null)
                {
                    response.Succeed(string.Format("Blog Post with ID {0} found.", post.PostID));
                    response.Entity = post;
                }
                else
                {
                    response.Fail(string.Format("Blog Post with ID {0} not found.", id));
                }
                return response;
            });
        }

        public RepositoryResponse Delete(int id)
        {
            return CatchError<RepositoryResponse>(() =>
            {
                var post = Find(id);
                var response = new RepositoryResponse();
                if (post != null)
                {
                    var comments = _context.Comments.Where(p => p.PostID == post.PostID);

                    //// first delete any related comments
                    //var commentRepo = (CommentRepository)_repository;
                    //var comments = commentRepo.All().Where(p => p.PostID == post.PostID);

                    foreach (var comment in comments.ToList())
                        CmsDbContext.ChangeState<Comment>(EntityState.Deleted, comment);

                    //_context.Posts.Attach(post);
                    //_context.Posts.Remove(post);
                    //_con

                    CmsDbContext.ChangeState<Post>(EntityState.Deleted, post);
                    CmsDbContext.Save();
                    response.Succeed(string.Format("Blog Post with ID {0} deleted.", id));
                }
                else
                {
                    response.Fail(string.Format("Blog Post with ID {0} not found.", id));
                }
                return response;
            });
        }
    }
}