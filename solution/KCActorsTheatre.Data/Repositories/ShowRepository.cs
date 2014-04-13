using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using KCActorsTheatre.Show;

namespace KCActorsTheatre.Data.Repositories
{
    public class ShowRepository : KCActorsTheatreRepositoryBase<ShowInfo>
    {
        private IKCActorsTheatreRepository _repository;
        private IKCActorsTheatreDbContext _context;
        public ShowRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository)
            : base(context, repository)
        {
            _context = context;
            _repository = repository;
        }

        protected override DbSet<ShowInfo> DbSet
        {
            get { return dbContext.Shows; }
        }

        public RepositoryResponse<ShowInfo> New(ShowInfo item)
        {
            return CatchError<RepositoryResponse<ShowInfo>>(() =>
            {
                DbSet.Add(item);
                CmsDbContext.Save();
                var response = new RepositoryResponse<ShowInfo>();
                response.Succeed("The new item was created successfully.");
                response.Entity = item;
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<ShowInfo>> FindForDisplay(IEnumerable<string> terms)
        {
            return CatchError<RepositoryResponse<IEnumerable<ShowInfo>>>(() =>
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
                var response = new RepositoryResponse<IEnumerable<ShowInfo>>();
                response.Succeed(string.Format("{0} items(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<ShowInfo>> FindForWebsite(string searchTerm)
        {
            return CatchError<RepositoryResponse<IEnumerable<ShowInfo>>>(() =>
            {
                var items = All(null, enableTracking: false)
                    .Where(p => p.ShowDate >= DateTime.UtcNow)
                ;
                if (searchTerm.Length > 0)
                {
                    items = items.Where(p => p.Title.IndexOf(searchTerm, StringComparison.CurrentCultureIgnoreCase) >= 0);
                }
                var response = new RepositoryResponse<IEnumerable<ShowInfo>>();
                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.ShowDate.Value).ThenBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<ShowInfo>> GetAll()
        {
            return CatchError<RepositoryResponse<IEnumerable<ShowInfo>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<ShowInfo>>();
                var items = All(null, enableTracking: false);
                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items.OrderByDescending(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<ShowInfo>> GetForWebsite(int? howMany = null, int? skip = null)
        {
            return CatchError<RepositoryResponse<IEnumerable<ShowInfo>>>(() =>
            {
                var events = All()
                    .Where(p => p.ShowDate.HasValue && p.ShowDate.Value >= DateTime.UtcNow)
                    .OrderBy(p => p.ShowDate.Value)
                    .ThenBy(p => p.DateCreated)
                    .ToList()
                ;
                if (skip.HasValue)
                    events = events.Skip(skip.Value).ToList();
                if (howMany.HasValue)
                    events = events.Take(howMany.Value).ToList();
                var response = new RepositoryResponse<IEnumerable<ShowInfo>>();
                response.Succeed(string.Format("{0} item(s) found.", events.Count()));
                response.Entity = events;
                return response;
            });
        }


        public RepositoryResponse<ShowInfo> GetSingle(int id)
        {
            return CatchError<RepositoryResponse<ShowInfo>>(() =>
            {
                var item = Single(a => a.ShowId == id, null, enableTracking:true);
                var response = new RepositoryResponse<ShowInfo>();
                if (item != null)
                {
                    response.Succeed(string.Format("Item with ID {0} found.", item.ShowId));
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
                    CmsDbContext.ChangeState<ShowInfo>(EntityState.Deleted, item);
                    CmsDbContext.Save();
                    response.Succeed(string.Format("Show with ID {0} deleted.", id));
                }
                else
                {
                    response.Fail(string.Format("Show with ID {0} not found.", id));
                }
                return response;
            });
        }
    }
}