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
using KCActorsTheatre.Organizations;

namespace KCActorsTheatre.Data.Repositories
{
    public class OrganizationRepository : KCActorsTheatreRepositoryBase<Organization>
    {
        private IKCActorsTheatreRepository _repository;
        private IKCActorsTheatreDbContext _context;
        public OrganizationRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository)
            : base(context, repository)
        {
            _context = context;
            _repository = repository;
        }

        protected override DbSet<Organization> DbSet
        {
            get { return dbContext.Organizations; }
        }

        public RepositoryResponse Delete(int id)
        {
            return CatchError<RepositoryResponse>(() =>
            {
                var item = DbSet
                    .Single(p => p.OrganizationID == id);

                var response = new RepositoryResponse();

                if (item != null)
                {
                    CmsDbContext.ChangeState<Organization>(EntityState.Deleted, item);
                    CmsDbContext.Save();
                    response.Succeed(string.Format("Organization with ID {0} deleted.", id));
                }
                else
                {
                    response.Fail(string.Format("Organization with ID {0} not found.", id));
                }
                return response;
            });
        }

        public RepositoryResponse<Organization> New(Organization item)
        {
            return CatchError<RepositoryResponse<Organization>>(() =>
            {
                DbSet.Add(item);
                CmsDbContext.Save();
                var response = new RepositoryResponse<Organization>();
                response.Succeed("The new Organization was created successfully.");
                response.Entity = item;
                return response;
            });
        }

        public RepositoryResponse<Organization> GetSingle(int id)
        {
            return CatchError<RepositoryResponse<Organization>>(() =>
            {
                var item = DbSet
                    .SingleOrDefault(p => p.OrganizationID == id)
                    ;

                var response = new RepositoryResponse<Organization>();
                if (item != null)
                {
                    response.Succeed(string.Format("Item with ID {0} found.", item.OrganizationID));
                    response.Entity = item;
                }
                else
                {
                    response.Fail(string.Format("Item with ID {0} not found.", id));
                }
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Organization>> GetAllByType(OrganizationType organizationType)
        {
            return CatchError<RepositoryResponse<IEnumerable<Organization>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<Organization>>();

                var items = DbSet
                    .Where(p => p.OrganizationType == organizationType)
                    .AsNoTracking()
                    ;


                if (items != null)
                {
                    response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                    response.Entity = items.OrderBy(a => a.DateCreated);
                }
                else
                {
                    response.Fail("No organizations found");
                }
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Organization>> FindForDisplay(IEnumerable<string> terms, OrganizationType organizationType)
        {
            return CatchError<RepositoryResponse<IEnumerable<Organization>>>(() =>
            {
                var items = All(null, enableTracking: false)
                    .Where(p => p.OrganizationType == organizationType);

                if (terms != null && terms.Count() > 0)
                {
                    items = items.Where(a =>
                        terms.Any(t =>
                            a.Name != null && a.Name.IndexOf(t, StringComparison.CurrentCultureIgnoreCase) >= 0
                        )
                    );
                }

                var response = new RepositoryResponse<IEnumerable<Organization>>();
                response.Succeed(string.Format("{0} items(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Organization>> Find(IEnumerable<string> terms, OrganizationType organizationType)
        {
            var resp = new RepositoryResponse<IEnumerable<Organization>>();
            try
            {
                var tags = this.All();
                resp.Entity = tags
                    .Where(p => p.OrganizationType == organizationType)
                    .Where(c => terms.Any(t => c.Name.ToLower().IndexOf(t.ToLower()) > -1))
                    .OrderBy(c => c.Name)
                    .ToList()
                ;
                resp.Succeed(string.Format("{0} {1} found.", resp.Entity.Count(), "organizations"));
            }
            catch (Exception ex)
            {
                resp.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return resp;
        }
    }
}