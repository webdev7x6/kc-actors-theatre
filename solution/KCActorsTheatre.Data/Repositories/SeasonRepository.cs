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
    public class SeasonRepository : KCActorsTheatreRepositoryBase<SeasonInfo>
    {
        private IKCActorsTheatreRepository _repository;
        private IKCActorsTheatreDbContext _context;
        public SeasonRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository)
            : base(context, repository)
        {
            _context = context;
            _repository = repository;
        }

        protected override DbSet<SeasonInfo> DbSet
        {
            get { return dbContext.Seasons; }
        }

        public RepositoryResponse<SeasonInfo> New(SeasonInfo item)
        {
            return CatchError<RepositoryResponse<SeasonInfo>>(() =>
            {
                DbSet.Add(item);
                CmsDbContext.Save();
                var response = new RepositoryResponse<SeasonInfo>();
                response.Succeed("The new item was created successfully.");
                response.Entity = item;
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<SeasonInfo>> FindForDisplay(IEnumerable<string> terms)
        {
            return CatchError<RepositoryResponse<IEnumerable<SeasonInfo>>>(() =>
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
                var response = new RepositoryResponse<IEnumerable<SeasonInfo>>();
                response.Succeed(string.Format("{0} items(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<SeasonInfo>> FindForWebsite(string searchTerm)
        {
            return CatchError<RepositoryResponse<IEnumerable<SeasonInfo>>>(() =>
                {
                    var items = All(null, enableTracking: false);
                if (searchTerm.Length > 0)
                {
                    items = items.Where(p => p.Title.IndexOf(searchTerm, StringComparison.CurrentCultureIgnoreCase) >= 0);
                }
                var response = new RepositoryResponse<IEnumerable<SeasonInfo>>();
                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<SeasonInfo>> GetAll()
        {
            return CatchError<RepositoryResponse<IEnumerable<SeasonInfo>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<SeasonInfo>>();
                var items = All(null, enableTracking: false);
                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items.OrderByDescending(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<SeasonInfo>> GetForWebsite(int? howMany = null, int? skip = null)
        {
            return CatchError<RepositoryResponse<IEnumerable<SeasonInfo>>>(() =>
            {
                var events = All()
                    .OrderBy(p => p.DateCreated)
                    .ToList()
                ;
                if (skip.HasValue)
                    events = events.Skip(skip.Value).ToList();
                if (howMany.HasValue)
                    events = events.Take(howMany.Value).ToList();
                var response = new RepositoryResponse<IEnumerable<SeasonInfo>>();
                response.Succeed(string.Format("{0} item(s) found.", events.Count()));
                response.Entity = events;
                return response;
            });
        }


        public RepositoryResponse<SeasonInfo> GetSingle(int id)
        {
            return CatchError<RepositoryResponse<SeasonInfo>>(() =>
            {
                var item = Single(a => a.SeasonID == id, null, enableTracking:true);
                var response = new RepositoryResponse<SeasonInfo>();
                if (item != null)
                {
                    response.Succeed(string.Format("Item with ID {0} found.", item.SeasonID));
                    response.Entity = item;
                }
                else
                {
                    response.Fail(string.Format("Item with ID {0} not found.", id));
                }
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<SeasonInfo>> GetPastSeasons()
        {
            return CatchError(() =>
            {
                var item = DbSet
                    .AsNoTracking()
                    .Where(p => !p.IsCurrent)
                    .OrderByDescending(p => p.DateCreated)
                    ;

                var response = new RepositoryResponse<IEnumerable<SeasonInfo>>();
                if (item != null)
                {
                    response.Succeed("Successfully retrieved past seasons");
                    response.Entity = item;
                }
                else
                {
                    response.Fail("No current season found.");
                }
                return response;
            });
        }

        public RepositoryResponse<SeasonInfo> GetCurrent()
        {
            return CatchError(() =>
            {
                var item = DbSet
                    .AsNoTracking()
                    .Include(p => p.Shows)
                    .FirstOrDefault(p => p.IsCurrent)
                    ;

                var response = new RepositoryResponse<SeasonInfo>();
                if (item != null)
                {
                    response.Succeed(string.Format("Current Season with ID {0} found.", item.SeasonID));
                    response.Entity = item;
                }
                else
                {
                    response.Fail("No current season found.");
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
                    CmsDbContext.ChangeState<SeasonInfo>(EntityState.Deleted, item);
                    CmsDbContext.Save();
                    response.Succeed(string.Format("Season with ID {0} deleted.", id));
                }
                else
                {
                    response.Fail(string.Format("Season with ID {0} not found.", id));
                }
                return response;
            });
        }
    }
}