using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using KCActorsTheatre.Announcements;
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
    public class AnnouncementRepository : KCActorsTheatreRepositoryBase<Announcement>
    {
        private IKCActorsTheatreRepository _repository;
        private IKCActorsTheatreDbContext _context;
        public AnnouncementRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository)
            : base(context, repository)
        {   
            _context = context;
            _repository = repository;
        }

        protected override DbSet<Announcement> DbSet
        {
            get { return dbContext.Announcements; }
        }

        //public RepositoryResponse<Article> New(Article article)
        //{
        //    return CatchError<RepositoryResponse<Article>>(() =>
        //    {
        //        DbSet.Add(article);
        //        CmsDbContext.Save();
        //        var response = new RepositoryResponse<Article>();
        //        response.Succeed("The new item was created successfully.");
        //        response.Entity = article;
        //        return response;
        //    });
        //}

        public RepositoryResponse<Announcement> New(Announcement announcement)
        {
            return CatchError<RepositoryResponse<Announcement>>(() =>
            {
                DbSet.Add(announcement);
                CmsDbContext.Save();
                var response = new RepositoryResponse<Announcement>();
                response.Succeed("The new announcement was created successfully.");
                response.Entity = announcement;
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Announcement>> FindForDisplay(IEnumerable<string> terms, AnnouncementType type)
        {
            return CatchError<RepositoryResponse<IEnumerable<Announcement>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<Announcement>>();

                var items = All(null, enableTracking:false)
                    .Where(p => p.AnnouncementType == type);

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

        public RepositoryResponse<IEnumerable<Announcement>> GetAllByType(AnnouncementType type)
        {
            return CatchError<RepositoryResponse<IEnumerable<Announcement>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<Announcement>>();

                var items = DbSet
                    .Where(p => p.AnnouncementType == type)
                    .AsNoTracking();

                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.Title);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Announcement>> GetForLandingPage(AnnouncementType type)
        {
            //return CatchError<RepositoryResponse<IEnumerable<Announcement>>>(() =>
            //{
                var response = new RepositoryResponse<IEnumerable<Announcement>>();

                var items = DbSet
                    .Where(p => 
                        p.DatePublished.HasValue && 
                        p.DatePublished.Value <= DateTime.UtcNow && 
                        (
                            (
                                p.DateExpired.HasValue && 
                                p.DateExpired.Value >= DateTime.UtcNow
                            ) 
                            || !p.DateExpired.HasValue
                        ) &&
                        p.DatePublished.Value.Month == DateTime.UtcNow.Month
                    )
                    .Where(p => p.AnnouncementType == type)
                    .AsNoTracking();

                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items.OrderByDescending(a => a.DatePublished.Value);
                return response;
            //});
        }

        /// <summary>
        /// returns true if is past publish date and not expired
        /// </summary>
        /// <param name="announcement"></param>
        /// <returns></returns>
        private bool IsPublished(Announcement announcement)
        {
            return announcement.DatePublished.HasValue && !IsExpired(announcement);
        }

        /// <summary>
        /// returns true if has an expiration date and that date has expired
        /// </summary>
        /// <param name="announcement"></param>
        /// <returns></returns>
        private bool IsExpired(Announcement announcement)
        {
            return
                (
                    announcement.DateExpired.HasValue &&
                    announcement.DateExpired.Value >= DateTime.UtcNow
                )
                || !announcement.DateExpired.HasValue;
        }

        /// <summary>
        /// returns true if is currently published and publish date matches current calendar month
        /// </summary>
        /// <param name="announcement"></param>
        /// <returns></returns>
        private bool IsForCurrentMonth(Announcement announcement)
        {
            return
                IsPublished(announcement) &&
                announcement.DatePublished.Value.Month == DateTime.UtcNow.Month;
        }

        public RepositoryResponse<Announcement> GetSingle(int id)
        {
            return CatchError<RepositoryResponse<Announcement>>(() =>
            {
                var item = Single(a => a.AnnouncementID == id, new[] { "Tags", }, enableTracking: true);
                var response = new RepositoryResponse<Announcement>();
                if (item != null)
                {
                    response.Succeed(string.Format("Item with ID {0} found.", item.AnnouncementID));
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
                    .Single(p => p.AnnouncementID == id);

                var response = new RepositoryResponse();

                if (item != null)
                {
                    // remove tags
                    var tags = item.Tags.ToList();
                    tags.ForEach(tag => item.Tags.Remove(tag));

                    CmsDbContext.ChangeState<Announcement>(EntityState.Deleted, item);
                    CmsDbContext.Save();
                    response.Succeed(string.Format("Announcement with ID {0} deleted.", id));
                }
                else
                {
                    response.Fail(string.Format("Announcement with ID {0} not found.", id));
                }
                return response;
            });
        }

        /// <summary>
        /// Adds a tag to an Announcement
        /// </summary>
        /// <param name="resourceID"></param>
        /// <param name="tagID"></param>
        /// <returns></returns>
        public RepositoryResponse<Tag> AddTag(int id, int tagID)
        {
            var resp = new RepositoryResponse<Tag>();
            try
            {
                var tag = _context.Tags
                    .Single(p => p.TagID == tagID);

                var item = this.Single(p => p.AnnouncementID == id, new string[] { "Tags" }, true);

                if (tag != null && item != null)
                {
                    item.Tags.Add(tag);
                    _context.Save();
                    resp.Entity = tag;
                    resp.Succeed(string.Format("Tag {0} added to Announcement {1}.", tag.Name, item.Title));
                }
                else
                {
                    resp.Fail(string.Format("Announcement {0} not found.", id));
                }
            }
            catch (Exception ex)
            {
                resp.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return resp;
        }

        /// <summary>
        /// removes a tag from an Announcement
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

                var item = this.Single(p => p.AnnouncementID == id, new string[] { "Tags" }, true);

                if (tag != null && item != null)
                {
                    item.Tags.Remove(tag);
                    _context.Save();
                    resp.Succeed(string.Format("Tag {0} removed from Announcement {1}.", tag.Name, item.Title));
                }
                else
                {
                    resp.Fail(string.Format("Announcement with id {0} not found.", id));
                }
            }
            catch (Exception ex)
            {
                resp.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return resp;
        }

        #region Website Methods

        public RepositoryResponse<IEnumerable<Announcement>> GetByTags(List<string> tags)
        {
            return CatchError<RepositoryResponse<IEnumerable<Announcement>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<Announcement>>();

                var announcements = DbSet
                    .Include(p => p.Tags)
                    .AsNoTracking();

                if (announcements != null && announcements.Count() > 0)
                {
                    announcements = announcements.Where(announcement =>
                        tags.Any(searchTag =>
                            announcement.Tags.Any(tag =>
                                tag.Name != null && tag.Name.IndexOf(searchTag) >= 0)
                        )
                    );
                }

                response.Succeed(string.Format("{0} resource(s) found.", announcements.Count()));

                response.Entity = announcements
                    .ToList()
                    .Where(p => p.IsPublished)
                    .OrderByDescending(p => p.DatePublished)
                    .ThenBy(p => p.Title)
                    ;

                return response;
            });
        }

        #endregion
    }
}