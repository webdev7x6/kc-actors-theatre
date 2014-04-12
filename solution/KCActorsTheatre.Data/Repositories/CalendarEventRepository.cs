using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using KCActorsTheatre.Calendar;
using KCActorsTheatre.Tags;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCActorsTheatre.Data.Repositories
{
    public class CalendarEventRepository : KCActorsTheatreRepositoryBase<CalendarEvent>
    {
        private IKCActorsTheatreRepository _repository;
        private IKCActorsTheatreDbContext _context;
        public CalendarEventRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository)
            : base(context, repository)
        {   
            _context = context;
            _repository = repository;
        }

        protected override DbSet<CalendarEvent> DbSet
        {
            get { return dbContext.CalendarEvents; }
        }

        public RepositoryResponse<CalendarEvent> New(CalendarEvent item)
        {
            return CatchError<RepositoryResponse<CalendarEvent>>(() =>
            {
                DbSet.Add(item);
                CmsDbContext.Save();
                var response = new RepositoryResponse<CalendarEvent>();
                response.Succeed("The new CalendarEvent was created successfully.");
                response.Entity = item;
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<CalendarEvent>> FindForDisplay(IEnumerable<string> terms)
        {
            return CatchError<RepositoryResponse<IEnumerable<CalendarEvent>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<CalendarEvent>>();

                var items = All(null, enableTracking:false);

                if (terms != null && terms.Count() > 0)
                {
                    items = items.Where(a =>
                        terms.Any(t =>
                            a.Title != null && a.Title.IndexOf(t, StringComparison.CurrentCultureIgnoreCase) >= 0
                        )
                    );
                }

                if (items != null)
                {
                    response.Succeed(string.Format("{0} items(s) found.", items.Count()));
                    response.Entity = items.OrderBy(a => a.Title);
                }
                else
                {
                    response.Fail("No items found");
                }

                return response;
            });
        }

        public RepositoryResponse<IEnumerable<CalendarEvent>> Find(IEnumerable<string> terms)
        {
            var resp = new RepositoryResponse<IEnumerable<CalendarEvent>>();
            try
            {
                var tags = this.All();
                resp.Entity = tags
                    .Where(c => terms.Any(t => c.Title.ToLower().IndexOf(t.ToLower()) > -1))
                    .OrderBy(c => c.Title)
                    .ToList()
                ;
                resp.Succeed(string.Format("{0} {1}(s) found.", resp.Entity.Count(), "calendar event"));
            }
            catch (Exception ex)
            {
                resp.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return resp;
        }

        public RepositoryResponse<IEnumerable<CalendarEvent>> GetAll()
        {
            return CatchError<RepositoryResponse<IEnumerable<CalendarEvent>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<CalendarEvent>>();

                var items = DbSet
                    .Include(p => p.EventCategory)
                    .Include(p => p.Tags)
                    .AsNoTracking()
                    ;

                if (items != null)
                {
                    response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                    response.Entity = items.OrderBy(a => a.Title);
                }
                else
                {
                    response.Fail("No calendar events found");
                }
                return response;
            });
        }

        public RepositoryResponse<CalendarEvent> GetSingle(int id)
        {
            return CatchError<RepositoryResponse<CalendarEvent>>(() =>
            {
                var item =DbSet
                    .Include(p => p.EventCategory)
                    .Include(p => p.Tags)
                    .Single(a => a.CalendarEventID == id)
                    ;

                var response = new RepositoryResponse<CalendarEvent>();
                if (item != null)
                {
                    response.Succeed(string.Format("Item with ID {0} found.", item.CalendarEventID));
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
                var item = DbSet
                    .Include(l => l.Tags)
                    .Single(p => p.CalendarEventID == id);

                var response = new RepositoryResponse();

                if (item != null)
                {
                    // remove tags
                    var tags = item.Tags.ToList();
                    tags.ForEach(tag => item.Tags.Remove(tag));

                    CmsDbContext.ChangeState<CalendarEvent>(EntityState.Deleted, item);
                    CmsDbContext.Save();
                    response.Succeed(string.Format("CalendarEvent with ID {0} deleted.", id));
                }
                else
                {
                    response.Fail(string.Format("CalendarEvent with ID {0} not found.", id));
                }
                return response;
            });
        }

        /// <summary>
        /// Adds a tag to a calendar event
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tagID"></param>
        /// <returns></returns>
        public RepositoryResponse<Tag> AddTag(int id, int tagID)
        {
            var resp = new RepositoryResponse<Tag>();
            try
            {
                var tag = _context.Tags
                    .Single(p => p.TagID == tagID);

                var item = this.Single(p => p.CalendarEventID == id, new string[] { "Tags" }, true);

                if (tag != null && item != null)
                {
                    item.Tags.Add(tag);
                    _context.Save();
                    resp.Entity = tag;
                    resp.Succeed(string.Format("Tag {0} added to calendar event {1}.", tag.Name, item.Title));
                }
                else
                {
                    resp.Fail(string.Format("calendar event {0} not found.", id));
                }
            }
            catch (Exception ex)
            {
                resp.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return resp;
        }

        /// <summary>
        /// removes a tag from a calendar event
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tagID"></param>
        /// <returns></returns>
        public RepositoryResponse RemoveTag(int id, int tagID)
        {
            var resp = new RepositoryResponse();
            try
            {
                var tag = _context.Tags
                    .Single(p => p.TagID == tagID);

                var item = this.Single(p => p.CalendarEventID == id, new string[] { "Tags" }, true);

                if (tag != null && item != null)
                {
                    item.Tags.Remove(tag);
                    _context.Save();
                    resp.Succeed(string.Format("Tag {0} removed from calendar event {1}.", tag.Name, item.Title));
                }
                else
                {
                    resp.Fail(string.Format("Tag with id {0} not found.", id));
                }
            }
            catch (Exception ex)
            {
                resp.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return resp;
        }
    }
}