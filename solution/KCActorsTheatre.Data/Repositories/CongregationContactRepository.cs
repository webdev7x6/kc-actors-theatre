using Clickfarm.AppFramework.Responses;
using Clickfarm.AppFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCActorsTheatre.Locations;

namespace KCActorsTheatre.Data.Repositories
{
    public class CongregationContactRepository : KCActorsTheatreRepositoryBase<CongregationContact>
    {
        private IKCActorsTheatreRepository _repository;
        private IKCActorsTheatreDbContext _context;
        public CongregationContactRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository)
            : base(context, repository)
        {
            _context = context;
            _repository = repository;
        }

        protected override DbSet<CongregationContact> DbSet
        {
            get { return dbContext.CongregationContacts; }
        }

        public RepositoryResponse<CongregationContact> New(CongregationContact item)
        {
            return CatchError<RepositoryResponse<CongregationContact>>(() =>
            {
                DbSet.Add(item);
                CmsDbContext.Save();
                var response = new RepositoryResponse<CongregationContact>();
                response.Succeed("The new item was created successfully.");
                response.Entity = item;
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
                    CmsDbContext.ChangeState<CongregationContact>(EntityState.Deleted, item);
                    CmsDbContext.Save();
                    response.Succeed(string.Format("CongregationContact with ID {0} deleted.", id));
                }
                else
                {
                    response.Fail(string.Format("CongregationContact with ID {0} not found.", id));
                }
                return response;
            });
        }

        public RepositoryResponse<CongregationContact> GetSingle(int id)
        {
            return CatchError<RepositoryResponse<CongregationContact>>(() =>
            {
                var item = Single(a => a.CongregationContactID == id, null, enableTracking: true);
                var response = new RepositoryResponse<CongregationContact>();
                if (item != null)
                {
                    response.Succeed(string.Format("Item with ID {0} found.", item.CongregationContactID));
                    response.Entity = item;
                }
                else
                {
                    response.Fail(string.Format("Item with ID {0} not found.", id));
                }
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<CongregationContact>> GetAll()
        {
            return CatchError<RepositoryResponse<IEnumerable<CongregationContact>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<CongregationContact>>();
                var items = All(null, enableTracking: false);
                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<CongregationContact>> FindForDisplay(IEnumerable<string> terms)
        {
            return CatchError<RepositoryResponse<IEnumerable<CongregationContact>>>(() =>
            {
                var items = All(null, enableTracking: false);
                if (terms != null && terms.Count() > 0)
                {
                    items = items.Where(a =>
                        terms.Any(t =>
                            a.Name != null && a.Name.IndexOf(t, StringComparison.CurrentCultureIgnoreCase) >= 0
                        )
                    );
                }
                var response = new RepositoryResponse<IEnumerable<CongregationContact>>();
                response.Succeed(string.Format("{0} items(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<CongregationContact>> Find(IEnumerable<string> terms)
        {
            var resp = new RepositoryResponse<IEnumerable<CongregationContact>>();
            try
            {
                var items = this.All();
                resp.Entity = items
                    .Where(c => terms.Any(t => c.Name.ToLower().IndexOf(t.ToLower()) > -1))
                    .OrderBy(c => c.Name)
                    .ToList()
                ;
                resp.Succeed(string.Format("{0} {1} found.", resp.Entity.Count(), "CongregationContacts"));
            }
            catch (Exception ex)
            {
                resp.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return resp;
        }

        public RepositoryResponse UpdateContactDisplayOrder(int[] contactIDs)
        {
            var response = new RepositoryResponse();

            try
            {
                for (int i = 0; i < contactIDs.Length; i++)
                {
                    var contactID = contactIDs[i];
                    var contact = this.Single(p => p.CongregationContactID == contactID);
                    contact.DisplayOrder = i;
                }

                CmsDbContext.Save();

                response.Succeed("contact display order successfully sorted");
            }
            catch (Exception ex)
            {
                response.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return response;
        }


    
    
    
    
    }
}
