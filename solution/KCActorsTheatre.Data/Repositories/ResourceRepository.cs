using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using KCActorsTheatre;
using KCActorsTheatre.Resources;
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
    public class ResourceRepository : KCActorsTheatreRepositoryBase<Resource>
    {
        private IKCActorsTheatreRepository _repository;
        private IKCActorsTheatreDbContext _context;
        public ResourceRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository)
            : base(context, repository)
        {   
            _context = context;
            _repository = repository;
        }
            
        protected override DbSet<Resource> DbSet
        {
            get { return dbContext.Resources; }
        }

        public RepositoryResponse<Resource> New(Resource resource)
        {
            return CatchError<RepositoryResponse<Resource>>(() =>
            {
                DbSet.Add(resource);
                CmsDbContext.Save();
                var response = new RepositoryResponse<Resource>();
                response.Succeed("The new item was created successfully.");
                response.Entity = resource;
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Resource>> FindForDisplay(IEnumerable<string> terms, ResourceType resourceType)
        {
            return CatchError<RepositoryResponse<IEnumerable<Resource>>>(() =>
            {
                var items = All(null, enableTracking: false)
                    .Where(p => p.ResourceType == resourceType);

                if (terms != null && terms.Count() > 0)
                {
                    items = items.Where(a =>
                        terms.Any(t =>
                            a.Title != null && a.Title.IndexOf(t, StringComparison.CurrentCultureIgnoreCase) >= 0
                        )
                    );
                }
                var response = new RepositoryResponse<IEnumerable<Resource>>();
                response.Succeed(string.Format("{0} items(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Resource>> GetAllByType(ResourceType ResourceType)
        {
            return CatchError<RepositoryResponse<IEnumerable<Resource>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<Resource>>();

                var items = DbSet
                    .Include(p => p.Tags)
                    .Where(p => p.ResourceType == ResourceType)
                    .AsNoTracking();

                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<Resource> GetSingle(int id)
        {
            return CatchError<RepositoryResponse<Resource>>(() =>
            {
                var item = Single(a => a.ResourceID == id, new[] { "Tags" }, enableTracking: true);
                var response = new RepositoryResponse<Resource>();
                if (item != null)
                {
                    response.Succeed(string.Format("Item with ID {0} found.", item.ResourceID));
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
                    .Single(p => p.ResourceID == id);

                var response = new RepositoryResponse();

                if (item != null)
                {
                    // remove tags
                    var tags = item.Tags.ToList();
                    tags.ForEach(tag => item.Tags.Remove(tag));

                    CmsDbContext.ChangeState<Resource>(EntityState.Deleted, item);
                    CmsDbContext.Save();
                    response.Succeed(string.Format("Resource with ID {0} deleted.", id));
                }
                else
                {
                    response.Fail(string.Format("Resource with ID {0} not found.", id));
                }
                return response;
            });
        }

        /// <summary>
        /// Adds a tag to a resource
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

                var item = this.Single(p => p.ResourceID == id, new string[] { "Tags" }, true);

                if (tag != null && item != null)
                {
                    item.Tags.Add(tag);
                    _context.Save();
                    resp.Entity = tag;
                    resp.Succeed(string.Format("Tag {0} added to Resource {1}.", tag.Name, item.Title));
                }
                else
                {
                    resp.Fail(string.Format("Resource {0} not found.", id));
                }
            }
            catch (Exception ex)
            {
                resp.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return resp;
        }

        /// <summary>
        /// removes a tag from a resource
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

                var item = this.Single(p => p.ResourceID == id, new string[] { "Tags" }, true);

                if (tag != null && item != null)
                {
                    item.Tags.Remove(tag);
                    _context.Save();
                    resp.Succeed(string.Format("Tag {0} removed from Resource {1}.", tag.Name, item.Title));
                }
                else
                {
                    resp.Fail(string.Format("Resource {0} not found.", id));
                }
            }
            catch (Exception ex)
            {
                resp.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return resp;
        }

        #region Website Methods

        public RepositoryResponse<IEnumerable<Resource>> GetByTags(List<string> tags)
        {
            return CatchError<RepositoryResponse<IEnumerable<Resource>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<Resource>>();

                var resources = DbSet
                    .Include(p => p.Tags)
                    .AsNoTracking();

                if (resources != null && resources.Count() > 0)
                {
                    resources = resources.Where(resource =>
                        tags.Any(searchTag =>
                            resource.Tags.Any(tag =>
                                tag.Name != null && tag.Name.IndexOf(searchTag) >= 0)
                        )
                    );
                }

                response.Succeed(string.Format("{0} resource(s) found.", resources.Count()));
                response.Entity = resources.OrderBy(a => a.DateCreated).ThenBy(p => p.Title);
                return response;
            });
        }

        #endregion

    }
}
