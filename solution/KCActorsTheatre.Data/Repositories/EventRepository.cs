using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using KCActorsTheatre.Calendar;

namespace KCActorsTheatre.Data.Repositories
{
    public class EventRepository : KCActorsTheatreRepositoryBase<Event>
    {
        private IKCActorsTheatreRepository _repository;
        private IKCActorsTheatreDbContext _context;
        public EventRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository)
            : base(context, repository)
        {
            _context = context;
            _repository = repository;
        }

        protected override DbSet<Event> DbSet
        {
            get { return dbContext.Events; }
        }

        public RepositoryResponse<Event> New(Event item)
        {
            return CatchError<RepositoryResponse<Event>>(() =>
            {
                DbSet.Add(item);
                CmsDbContext.Save();
                var response = new RepositoryResponse<Event>();
                response.Succeed("The new item was created successfully.");
                response.Entity = item;
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Event>> FindForDisplay(IEnumerable<string> terms)
        {
            return CatchError<RepositoryResponse<IEnumerable<Event>>>(() =>
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
                var response = new RepositoryResponse<IEnumerable<Event>>();
                response.Succeed(string.Format("{0} items(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Event>> FindForWebsite(string searchTerm)
        {
            return CatchError<RepositoryResponse<IEnumerable<Event>>>(() =>
            {
                var items = All(null, enableTracking: false)
                    .Where(p => p.StartDate >= DateTime.UtcNow)
                ;
                if (searchTerm.Length > 0)
                {
                    items = items.Where(p => p.Title.IndexOf(searchTerm, StringComparison.CurrentCultureIgnoreCase) >= 0);
                }
                var response = new RepositoryResponse<IEnumerable<Event>>();
                response.Succeed(string.Format("{0} calendar event(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.StartDate.Value).ThenBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Event>> GetAll()
        {
            return CatchError<RepositoryResponse<IEnumerable<Event>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<Event>>();
                var items = All(null, enableTracking: false);
                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items.OrderByDescending(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Event>> GetForWebsite(int? howMany = null, int? skip = null)
        {
            return CatchError<RepositoryResponse<IEnumerable<Event>>>(() =>
            {
                var events = All()
                    .Where(p => p.StartDate.HasValue && p.EndDate.HasValue && p.StartDate.Value >= DateTime.UtcNow)
                    .OrderBy(p => p.StartDate.Value)
                    .ThenBy(p => p.DateCreated)
                    .ToList()
                ;
                if (skip.HasValue)
                    events = events.Skip(skip.Value).ToList();
                if (howMany.HasValue)
                    events = events.Take(howMany.Value).ToList();
                var response = new RepositoryResponse<IEnumerable<Event>>();
                response.Succeed(string.Format("{0} calendar event(s) found.", events.Count()));
                response.Entity = events;
                return response;
            });
        }


        public RepositoryResponse<Event> GetSingle(int id)
        {
            return CatchError<RepositoryResponse<Event>>(() =>
            {
                var item = Single(a => a.EventID == id, null, enableTracking:true);
                var response = new RepositoryResponse<Event>();
                if (item != null)
                {
                    response.Succeed(string.Format("Item with ID {0} found.", item.EventID));
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
                    CmsDbContext.ChangeState<Event>(EntityState.Deleted, item);
                    CmsDbContext.Save();
                    response.Succeed(string.Format("Event with ID {0} deleted.", id));
                }
                else
                {
                    response.Fail(string.Format("Event with ID {0} not found.", id));
                }
                return response;
            });
        }
    }
}