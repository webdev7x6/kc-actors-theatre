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
    public class AuthorRepository : KCActorsTheatreRepositoryBase<Author>
    {
        private IKCActorsTheatreRepository _repository;
        private IKCActorsTheatreDbContext _context;
        public AuthorRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository) : base(context, repository)
        {
            _context = context;
            _repository = repository;
        }

        protected override DbSet<Author> DbSet
        {
            get { return dbContext.Authors; }
        }

        public RepositoryResponse<Author> New(Author author)
        {
            return CatchError<RepositoryResponse<Author>>(() =>
            {
                DbSet.Add(author);
                CmsDbContext.Save();
                var response = new RepositoryResponse<Author>();
                response.Succeed("The new item was created successfully.");
                response.Entity = author;
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Author>> FindForDisplay(IEnumerable<string> terms)
        {
            return CatchError<RepositoryResponse<IEnumerable<Author>>>(() =>
            {
                var items = All(null, enableTracking: false);
                if (terms != null && terms.Count() > 0)
                {
                    items = items.Where(a =>
                        terms.Any(t =>
                            a.Name != null && a.Name.IndexOf(t, StringComparison.CurrentCultureIgnoreCase) >= 0
                        )
                    );
                }
                var response = new RepositoryResponse<IEnumerable<Author>>();
                response.Succeed(string.Format("{0} items(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Author>> GetAll()
        {
            return CatchError<RepositoryResponse<IEnumerable<Author>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<Author>>();
                var items = All(null, enableTracking: false);
                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<Author> GetSingle(int id)
        {
            return CatchError<RepositoryResponse<Author>>(() =>
            {
                var item = Single(a => a.AuthorID == id, null, enableTracking:true);
                var response = new RepositoryResponse<Author>();
                if (item != null)
                {
                    response.Succeed(string.Format("Item with ID {0} found.", item.AuthorID));
                    response.Entity = item;
                }
                else
                {
                    response.Fail(string.Format("Item with ID {0} not found.", id));
                }
                return response;
            });
        }

        public RepositoryResponse Delete(int id)
        {
            return CatchError<RepositoryResponse>(() =>
            {
                var item = Find(id);
                var response = new RepositoryResponse();
                if (item != null)
                {
                    // remove author from related posts
                    foreach (var post in _context.Posts.Where(p => p.AuthorID == item.AuthorID))
                    {
                        post.AuthorID = null;
                        CmsDbContext.ChangeState<Post>(EntityState.Modified, post);
                    }

                    CmsDbContext.ChangeState<Author>(EntityState.Deleted, item);
                    CmsDbContext.Save();
                    response.Succeed(string.Format("Author with ID {0} deleted.", id));
                }
                else
                {
                    response.Fail(string.Format("Author with ID {0} not found.", id));
                }
                return response;
            });
        }
    }
}