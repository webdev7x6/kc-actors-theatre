using Clickfarm.AppFramework.Responses;
using KCActorsTheatre;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clickfarm.AppFramework.Extensions;
using KCActorsTheatre.Resources;
using System.Data;
using KCActorsTheatre.Tags;
using KCActorsTheatre.Cms.ContentTypes;

namespace KCActorsTheatre.Data.Repositories
{
    public class TagRepository : KCActorsTheatreRepositoryBase<Tag>
    {
        private IKCActorsTheatreRepository _repository;
        private IKCActorsTheatreDbContext _context;
        public TagRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository)
            : base(context, repository)
        {
            _context = context;
            _repository = repository;
        }

        protected override DbSet<Tag> DbSet
        {
            get { return dbContext.Tags; }
        }

        public RepositoryResponse Delete(int id)
        {
            return CatchError<RepositoryResponse>(() =>
            {
                var item = DbSet
                    .Include(p => p.Resources)
                    .Include(p => p.Announcements)
                    .Include(p => p.MissionStories)
                    .Single(p => p.TagID == id);

                var response = new RepositoryResponse();

                if (item != null)
                {
                    // remove tag from resources
                    var resources = item.Resources.ToList();
                    resources.ForEach(p => p.Tags.Remove(item));

                    // remove tag from announcements
                    var articles = item.Announcements.ToList();
                    articles.ForEach(p => p.Tags.Remove(item));

                    // remove tag from mission stories
                    var missionStories = item.MissionStories.ToList();
                    missionStories.ForEach(p => p.Tags.Remove(item));

                    CmsDbContext.ChangeState<Tag>(EntityState.Deleted, item);
                    CmsDbContext.Save();
                    response.Succeed(string.Format("Tag with ID {0} deleted.", id));
                }
                else
                {
                    response.Fail(string.Format("Tag with ID {0} not found.", id));
                }
                return response;
            });
        }

        public RepositoryResponse<Tag> New(Tag item)
        {
            return CatchError<RepositoryResponse<Tag>>(() =>
            {
                DbSet.Add(item);
                CmsDbContext.Save();
                var response = new RepositoryResponse<Tag>();
                response.Succeed("The new item was created successfully.");
                response.Entity = item;
                return response;
            });
        }

        public RepositoryResponse<Tag> GetSingle(int id)
        {
            return CatchError<RepositoryResponse<Tag>>(() =>
            {
                var item = DbSet
                    .Include(p => p.Resources)
                    .Include(p => p.Announcements)
                    .Include(p => p.MissionStories)
                    .SingleOrDefault(p => p.TagID == id)
                    ;

                var response = new RepositoryResponse<Tag>();
                if (item != null)
                {
                    response.Succeed(string.Format("Item with ID {0} found.", item.TagID));
                    response.Entity = item;
                }
                else
                {
                    response.Fail(string.Format("Item with ID {0} not found.", id));
                }
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Tag>> GetAllByType(TagType tagType)
        {
            return CatchError<RepositoryResponse<IEnumerable<Tag>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<Tag>>();

                var items = DbSet
                    .Include(p => p.Resources)
                    .Include(p => p.Announcements)
                    .Include(p => p.MissionStories)
                    .Where(p => p.TagType == tagType)
                    .AsNoTracking()
                    ;


                if (items != null)
                {
                    response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                    response.Entity = items.OrderBy(a => a.DateCreated);
                }
                else
                {
                    response.Fail("No tags found");
                }
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Tag>> FindForDisplay(IEnumerable<string> terms, TagType tagType)
        {
            return CatchError<RepositoryResponse<IEnumerable<Tag>>>(() =>
            {
                var items = All(null, enableTracking: false)
                    .Where(p => p.TagType == tagType);

                if (terms != null && terms.Count() > 0)
                {
                    items = items.Where(a =>
                        terms.Any(t =>
                            a.Name != null && a.Name.IndexOf(t, StringComparison.CurrentCultureIgnoreCase) >= 0
                        )
                    );
                }

                var response = new RepositoryResponse<IEnumerable<Tag>>();
                response.Succeed(string.Format("{0} items(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Tag>> Find(IEnumerable<string> terms, TagType tagType)
        {
            var resp = new RepositoryResponse<IEnumerable<Tag>>();
            try
            {
                var tags = this.All();
                resp.Entity = tags
                    .Where(p => p.TagType == tagType)
                    .Where(c => terms.Any(t => c.Name.ToLower().IndexOf(t.ToLower()) > -1))
                    .OrderBy(c => c.Name)
                    .ToList()
                ;
                resp.Succeed(string.Format("{0} {1} found.", resp.Entity.Count(), "Tags"));
            }
            catch (Exception ex)
            {
                resp.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return resp;
        }


        #region CMS Widget Tagging

        public RepositoryResponse<IEnumerable<Tag>> GetWidgetTags(int contentID)
        {
            return CatchError<RepositoryResponse<IEnumerable<Tag>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<Tag>>();

                var content = CmsDbContext.Content
                    .OfType<TaggedWidgetContent>()
                    .Include(p => p.Tags)
                    .SingleOrDefault(p => p.ContentID == contentID)
                    ;

                if (content != null && content.Tags != null)
                {
                    response.Succeed(string.Format("{0} tags(s) found.", content.Tags.Count()));
                    response.Entity = content.Tags.OrderBy(a => a.Name);
                }
                else
                {
                    response.Fail("No tags found");
                }
                return response;
            });
        }

        /// <summary>
        /// Adds a tag to a tagged widget
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tagID"></param>
        /// <returns></returns>
        public RepositoryResponse<Tag> AddWidgetTag(int contentID, int tagID)
        {
            var response = new RepositoryResponse<Tag>();
            try
            {
                var content = CmsDbContext.Content
                    .OfType<TaggedWidgetContent>()
                    .Include(p => p.Tags)
                    .SingleOrDefault(p => p.ContentID == contentID)
                    ;

                var tag = _context.Tags
                    .SingleOrDefault(p => p.TagID == tagID);

                if (tag != null && content != null)
                {
                    content.Tags.Add(tag);
                    _context.Save();
                    response.Entity = tag;
                    response.Succeed(string.Format("Tag {0} added to TaggedWidget {1}.", tag.Name, content.Description));
                }
                else
                {
                    response.Fail(string.Format("TaggedWidgetContent {0} not found.", contentID));
                }
            }
            catch (Exception ex)
            {
                response.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return response;
        }

        /// <summary>
        /// removes a tag from a tagged widget
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tagID"></param>
        /// <returns></returns>
        public RepositoryResponse<Tag> RemoveWidgetTag(int contentID, int tagID)
        {
            var response = new RepositoryResponse<Tag>();
            try
            {
                var content = CmsDbContext.Content
                    .OfType<TaggedWidgetContent>()
                    .Include(p => p.Tags)
                    .SingleOrDefault(p => p.ContentID == contentID)
                    ;

                var tag = _context.Tags
                    .Include(p => p.TaggedWidgets)
                    .SingleOrDefault(p => p.TagID == tagID);

                if (tag != null && content != null)
                {
                    content.Tags.Remove(tag);
                    _context.Save();
                    response.Entity = tag;
                    response.Succeed(string.Format("Tag {0} removed from Tagged Widget {1}.", tag.Name, content.Description));
                }
                else
                {
                    response.Fail(string.Format("TaggedWidgetContent {0} not found.", contentID));
                }
            }
            catch (Exception ex)
            {
                response.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return response;
        }


        #endregion


    }
}