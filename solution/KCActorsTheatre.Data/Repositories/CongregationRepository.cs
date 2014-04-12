using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using KCActorsTheatre;
using KCActorsTheatre.Locations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCActorsTheatre.Data.Repositories
{
    public class CongregationRepository : KCActorsTheatreRepositoryBase<Congregation>
    {
        private IKCActorsTheatreRepository _repository;
        private IKCActorsTheatreDbContext _context;
        public CongregationRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository)
            : base(context, repository)
        {   
            _context = context;
            _repository = repository;
        }
            
        protected override DbSet<Congregation> DbSet
        {
            get { return dbContext.Congregations; }
        }

        public RepositoryResponse<Congregation> New(Congregation congregation)
        {
            //return CatchError<RepositoryResponse<Congregation>>(() =>
            //{
                DbSet.Add(congregation);
                CmsDbContext.Save();
                var response = new RepositoryResponse<Congregation>();
                response.Succeed("The new item was created successfully.");
                response.Entity = congregation;
                return response;
            //});
        }

        public RepositoryResponse<IEnumerable<Congregation>> FindForDisplay(IEnumerable<string> terms)
        {
            return CatchError<RepositoryResponse<IEnumerable<Congregation>>>(() =>
            {
                var items = All(new[] { "CongregationContacts", "MissionCenter" }, enableTracking: false);
                if (terms != null && terms.Count() > 0)
                {
                    items = items.Where(a =>
                        terms.Any(t =>
                            a.Name != null && a.Name.IndexOf(t, StringComparison.CurrentCultureIgnoreCase) >= 0
                        )
                    );
                }
                var response = new RepositoryResponse<IEnumerable<Congregation>>();
                response.Succeed(string.Format("{0} items(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Congregation>> GetAll()
        {
            return CatchError<RepositoryResponse<IEnumerable<Congregation>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<Congregation>>();
                var items = All(new[] { "CongregationContacts", "MissionCenter" }, enableTracking: false);
                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<Congregation> GetSingle(int id)
        {
            return CatchError<RepositoryResponse<Congregation>>(() =>
            {
                var item = Single(a => a.CongregationID == id, new[] { "CongregationContacts", "MissionCenter" }, enableTracking: true);
                var response = new RepositoryResponse<Congregation>();
                if (item != null)
                {
                    response.Succeed(string.Format("Item with ID {0} found.", item.CongregationID));
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
                    CmsDbContext.ChangeState<Congregation>(EntityState.Deleted, item);
                    CmsDbContext.Save();
                    response.Succeed(string.Format("Congregation with ID {0} deleted.", id));
                }
                else
                {
                    response.Fail(string.Format("Congregation with ID {0} not found.", id));
                }
                return response;
            });
        }

        /// <summary>
        /// Adds a congregation contact to a congregation
        /// </summary>
        /// <param name="congregationID"></param>
        /// <param name="congregationContactID"></param>
        /// <returns></returns>
        public RepositoryResponse<CongregationContact> AddCongregationContact(int congregationID, int congregationContactID)
        {
            var resp = new RepositoryResponse<CongregationContact>();
            try
            {
                var congregationContact = _context.CongregationContacts
                    .Single(p => p.CongregationContactID == congregationContactID);

                var congregation = this.Single(p => p.CongregationID == congregationID, new string[] { "CongregationContacts", "MissionCenter" }, true);

                if (congregationContact != null && congregation != null)
                {
                    congregation.CongregationContacts.Add(congregationContact);
                    _context.Save();
                    resp.Entity = congregationContact;
                    resp.Succeed(string.Format("Congregation Contact {0} added to Congregation {1}.", congregationContact.Name, congregation.Name));
                }
                else
                {
                    resp.Fail(string.Format("Congregation {0} not found.", congregationID));
                }
            }
            catch (Exception ex)
            {
                resp.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return resp;
        }

        /// <summary>
        /// removes a congregation contact from a congregation
        /// </summary>
        /// <param name="resourceID"></param>
        /// <param name="tagID"></param>
        /// <returns></returns>
        public RepositoryResponse RemoveCongregationContact(int congregationID, int congregationContactID)
        {
            var resp = new RepositoryResponse();
            try
            {
                var congregationContact = _context.CongregationContacts
                    .Single(p => p.CongregationContactID == congregationContactID);

                var congregation = this.Single(p => p.CongregationID == congregationID, new string[] { "CongregationContacts", "MissionCenter" }, true);

                if (congregationContact != null && congregation != null)
                {
                    congregation.CongregationContacts.Remove(congregationContact);
                    _context.Save();
                    resp.Succeed(string.Format("Congregation Contact {0} removed from Congregation {1}.", congregationContact.Name, congregation.Name));
                }
                else
                {
                    resp.Fail(string.Format("Congregation {0} not found.", congregationID));
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
