using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using KCActorsTheatre.Contract;

namespace KCActorsTheatre.Data.Repositories
{
    public class NewsArticleRepository : KCActorsTheatreRepositoryBase<Article>
    {
        private IKCActorsTheatreRepository _repository;
        private IKCActorsTheatreDbContext _context;
        public NewsArticleRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository)
            : base(context, repository)
        {
            _context = context;
            _repository = repository;
        }

        protected override DbSet<Article> DbSet
        {
            get { return dbContext.NewsArticles; }
        }

        public RepositoryResponse<Article> New(Article item)
        {
            return CatchError<RepositoryResponse<Article>>(() =>
            {
                DbSet.Add(item);
                CmsDbContext.Save();
                var response = new RepositoryResponse<Article>();
                response.Succeed("The new item was created successfully.");
                response.Entity = item;
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Article>> FindForDisplay(IEnumerable<string> terms)
        {
            return CatchError<RepositoryResponse<IEnumerable<Article>>>(() =>
            {
                var items = All(null, enableTracking: false);
                if (terms != null && terms.Count() > 0)
                {
                    items = items.Where(a =>
                        terms.Any(t =>
                            a.Title != null && a.Title.IndexOf(t, StringComparison.CurrentCultureIgnoreCase) >= 0
                        )
                    );
                }
                var response = new RepositoryResponse<IEnumerable<Article>>();
                response.Succeed(string.Format("{0} items(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Article>> FindForWebsite(string searchTerm)
        {
            return CatchError<RepositoryResponse<IEnumerable<Article>>>(() =>
            {
                var items = All(null, enableTracking: false)
                    .Where(p => p.IsPublished == true)
                ;
                if (searchTerm.Length > 0)
                {
                    items = items.Where(p => p.Title.IndexOf(searchTerm, StringComparison.CurrentCultureIgnoreCase) >= 0);
                }
                var response = new RepositoryResponse<IEnumerable<Article>>();
                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items.OrderByDescending(a => a.ArticleDate.Value);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Article>> GetAll()
        {
            return CatchError<RepositoryResponse<IEnumerable<Article>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<Article>>();
                var items = All(null, enableTracking: false);
                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items.OrderByDescending(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Article>> GetForWebsite(int? howMany = null, int? skip = null)
        {
            return CatchError<RepositoryResponse<IEnumerable<Article>>>(() =>
            {
                var events = All()
                    .Where(p => p.IsPublished == true)
                    .OrderByDescending(p => p.ArticleDate.Value)
                    .ThenBy(p => p.Title)
                    .ToList()
                ;
                if (skip.HasValue)
                    events = events.Skip(skip.Value).ToList();
                if (howMany.HasValue)
                    events = events.Take(howMany.Value).ToList();
                var response = new RepositoryResponse<IEnumerable<Article>>();
                response.Succeed(string.Format("{0} item(s) found.", events.Count()));
                response.Entity = events;
                return response;
            });
        }


        public RepositoryResponse<Article> GetSingle(int id)
        {
            return CatchError<RepositoryResponse<Article>>(() =>
            {
                var item = Single(a => a.ArticleID == id, null, enableTracking:true);
                var response = new RepositoryResponse<Article>();
                if (item != null)
                {
                    response.Succeed(string.Format("Item with ID {0} found.", item.ArticleID));
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
                    CmsDbContext.ChangeState<Article>(EntityState.Deleted, item);
                    CmsDbContext.Save();
                    response.Succeed(string.Format("Article with ID {0} deleted.", id));
                }
                else
                {
                    response.Fail(string.Format("Article with ID {0} not found.", id));
                }
                return response;
            });
        }
    }
}