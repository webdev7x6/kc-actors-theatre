using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using KCActorsTheatre.MissionStories;
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
    public class MissionStoryRepository : KCActorsTheatreRepositoryBase<MissionStory>
    {
        private IKCActorsTheatreRepository _repository;
        private IKCActorsTheatreDbContext _context;
        public MissionStoryRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository)
            : base(context, repository)
        {   
            _context = context;
            _repository = repository;
        }

        protected override DbSet<MissionStory> DbSet
        {
            get { return dbContext.MissionStories; }
        }

        public RepositoryResponse<MissionStory> New(MissionStory item)
        {
            return CatchError<RepositoryResponse<MissionStory>>(() =>
            {
                DbSet.Add(item);
                CmsDbContext.Save();
                var response = new RepositoryResponse<MissionStory>();
                response.Succeed("The new MissionStory was created successfully.");
                response.Entity = item;
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<MissionStory>> FindForDisplay(IEnumerable<string> terms)
        {
            return CatchError<RepositoryResponse<IEnumerable<MissionStory>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<MissionStory>>();

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

        public RepositoryResponse<IEnumerable<MissionStory>> GetAll()
        {
            return CatchError<RepositoryResponse<IEnumerable<MissionStory>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<MissionStory>>();

                var items = DbSet
                    .AsNoTracking();

                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.Title);
                return response;
            });
        }

        public RepositoryResponse<MissionStory> GetSingle(int id)
        {
            return CatchError<RepositoryResponse<MissionStory>>(() =>
            {
                var item = Single(a => a.MissionStoryID == id, new[] { "Tags", }, enableTracking: true);
                var response = new RepositoryResponse<MissionStory>();
                if (item != null)
                {
                    response.Succeed(string.Format("Item with ID {0} found.", item.MissionStoryID));
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
                    .Single(p => p.MissionStoryID == id);

                var response = new RepositoryResponse();

                if (item != null)
                {
                    // remove tags
                    var tags = item.Tags.ToList();
                    tags.ForEach(tag => item.Tags.Remove(tag));

                    CmsDbContext.ChangeState<MissionStory>(EntityState.Deleted, item);
                    CmsDbContext.Save();
                    response.Succeed(string.Format("MissionStory with ID {0} deleted.", id));
                }
                else
                {
                    response.Fail(string.Format("MissionStory with ID {0} not found.", id));
                }
                return response;
            });
        }

        /// <summary>
        /// Adds a tag to an MissionStory
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

                var item = this.Single(p => p.MissionStoryID == id, new string[] { "Tags" }, true);

                if (tag != null && item != null)
                {
                    item.Tags.Add(tag);
                    _context.Save();
                    resp.Entity = tag;
                    resp.Succeed(string.Format("Tag {0} added to MissionStory {1}.", tag.Name, item.Title));
                }
                else
                {
                    resp.Fail(string.Format("MissionStory {0} not found.", id));
                }
            }
            catch (Exception ex)
            {
                resp.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return resp;
        }

        /// <summary>
        /// removes a tag from an MissionStory
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

                var item = this.Single(p => p.MissionStoryID == id, new string[] { "Tags" }, true);

                if (tag != null && item != null)
                {
                    item.Tags.Remove(tag);
                    _context.Save();
                    resp.Succeed(string.Format("Tag {0} removed from MissionStory {1}.", tag.Name, item.Title));
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

        public RepositoryResponse UpdateMissionStoriesDisplayOrder(int[] missionStoryIDs)
        {
            var response = new RepositoryResponse();
            try
            {
                for (int i = 0; i < missionStoryIDs.Length; i++)
                {
                    var missionStoryID = missionStoryIDs[i];
                    var missionStory = DbSet.FirstOrDefault(p => p.MissionStoryID == missionStoryID);
                    if (missionStory != null)
                        missionStory.DisplayOrder = i;
                }

                CmsDbContext.Save();

                response.Succeed("mission stories display order successfully sorted");
            }
            catch (Exception ex)
            {
                response.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return response;
        }

        public RepositoryResponse<IEnumerable<MissionStory>> GetTagged(IEnumerable<string> searchTags)
        {
            return CatchError<RepositoryResponse<IEnumerable<MissionStory>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<MissionStory>>();

                var missionStories = DbSet
                    .Include(p => p.Tags)
                    .AsNoTracking()
                ;

                if (searchTags != null && searchTags.Count() > 0)
                {
                    missionStories = missionStories.Where(missionStory =>
                        searchTags.Any(searchTag =>
                            missionStory.Tags.Any(tag =>
                                tag.Name != null && tag.Name.IndexOf(searchTag) >= 0)
                        )
                    );
                }

                if (missionStories != null)
                {
                    response.Succeed(string.Format("{0} item(s) found.", missionStories.Count()));
                    response.Entity = missionStories
                        .OrderByDescending(a => a.DatePublished)
                        .ThenBy(a => a.Title)
                    ;
                }
                else
                {
                    response.Fail("No items found");
                }

                return response;
            });
        }

        #region Website Methods

        public RepositoryResponse<IEnumerable<MissionStory>> GetHomePageMissionStories()
        {
            return CatchError<RepositoryResponse<IEnumerable<MissionStory>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<MissionStory>>();

                var items = GetTagged(new string[] { "home page" })
                    .Entity
                    .Where(p => p.IsPublished)
                    .OrderBy(p => p.DisplayOrder)
                    ;

                if (items != null)
                {
                    response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                    response.Entity = items;
                }
                else
                {
                    response.Fail("Mission Story not found.");
                }

                return response;
            });
        }

        public RepositoryResponse<MissionStory> GetForDetailPage(int id)
        {
            return CatchError<RepositoryResponse<MissionStory>>(() =>
            {
                var response = new RepositoryResponse<MissionStory>();

                var item = DbSet
                    .AsNoTracking()
                    .Where(p => p.DatePublished.HasValue && p.DatePublished.Value < DateTime.UtcNow)
                    .FirstOrDefault(p => p.MissionStoryID == id)
                    ;

                if (item != null)
                {
                    response.Succeed(string.Format("Successfully retrieved mission story: {0}", item.Title));
                    response.Entity = item;
                }
                else
                {
                    response.Fail("Mission Story not found.");
                }

                return response;
            });
        }

        public RepositoryResponse<IEnumerable<MissionStory>> GetForLandingPage()
        {
            return CatchError<RepositoryResponse<IEnumerable<MissionStory>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<MissionStory>>();

                var items = DbSet
                    .Where(p => p.DatePublished.HasValue && p.DatePublished.Value < DateTime.UtcNow)
                    .OrderByDescending(p => p.DatePublished.Value)
                    .AsNoTracking();

                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items;
                return response;
            });
        }

        #endregion
    }
}