using Clickfarm.AppFramework.Responses;
using KCActorsTheatre;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clickfarm.AppFramework.Extensions;
using System.Data;
using KCActorsTheatre.Locations;
using KCActorsTheatre.Staff;

namespace KCActorsTheatre.Data.Repositories
{
    public class MissionFieldRepository : KCActorsTheatreRepositoryBase<MissionField>
    {
        private IKCActorsTheatreRepository _repository;
        private IKCActorsTheatreDbContext _context;
        public MissionFieldRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository)
            : base(context, repository)
        {
            _context = context;
            _repository = repository;
        }

        protected override DbSet<MissionField> DbSet
        {
            get { return dbContext.MissionFields; }
        }

        public RepositoryResponse<MissionField> New(MissionField item)
        {
            return CatchError<RepositoryResponse<MissionField>>(() =>
            {
                DbSet.Add(item);
                CmsDbContext.Save();

                var response = new RepositoryResponse<MissionField>();

                var returnItem = DbSet
                    .SingleOrDefault(p => p.MissionFieldID == item.MissionFieldID);

                response.Succeed("The new item was created successfully.");
                response.Entity = returnItem;
                return response;
            });
        }

        public RepositoryResponse<MissionField> GetSingle(int id)
        {
            return CatchError<RepositoryResponse<MissionField>>(() =>
            {
                var item = Single(a => a.MissionFieldID == id, new string[] { "MissionCenters", "RoleDefinitions", "RoleDefinitions.StaffMember" }, enableTracking: true);
                var response = new RepositoryResponse<MissionField>();
                if (item != null)
                {
                    response.Succeed(string.Format("Item with ID {0} found.", item.MissionFieldID));
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
                    .Include(p => p.RoleDefinitions)
                    .SingleOrDefault(p => p.MissionFieldID == id);

                var response = new RepositoryResponse();

                if (item != null)
                {
                    // delete RoleDefinitions
                    var roleDefinitions = item.RoleDefinitions.ToList();
                    roleDefinitions.ForEach(rd => CmsDbContext.ChangeState<RoleDefinition>(EntityState.Deleted, rd));

                    CmsDbContext.ChangeState<MissionField>(EntityState.Deleted, item);
                    CmsDbContext.Save();

                    response.Succeed(string.Format("MissionField with ID {0} deleted.", id));
                }
                else
                {
                    response.Fail(string.Format("MissionField with ID {0} not found.", id));
                }
                return response;
            });
        }

        public RepositoryResponse DeleteRole(int missionFieldID, int roleDefinitionID)
        {
            return CatchError<RepositoryResponse>(() =>
            {
            var response = new RepositoryResponse();

            var item = DbSet
                .Include("RoleDefinitions")
                .SingleOrDefault(p => p.MissionFieldID == missionFieldID);

            if (item != null)
            {
                var role = item.RoleDefinitions.SingleOrDefault(p => p.RoleDefinitionID == roleDefinitionID);

                if (role != null)
                {
                    CmsDbContext.ChangeState<RoleDefinition>(EntityState.Deleted, role);
                    CmsDbContext.Save();
                }

                response.Succeed(string.Format("Role definition with ID {0} deleted.", roleDefinitionID));
            }
            else
            {
                response.Fail(string.Format("Role definition with ID {0} not found.", roleDefinitionID));
            }
            return response;
            });
        }


        public RepositoryResponse<IEnumerable<MissionField>> GetAll()
        {
            return CatchError<RepositoryResponse<IEnumerable<MissionField>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<MissionField>>();
                var items = All(new string[] { "MissionCenters", "RoleDefinitions", "RoleDefinitions.StaffMember" }, enableTracking: false);
                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.Name);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<MissionField>> FindForDisplay(IEnumerable<string> terms)
        {
            return CatchError<RepositoryResponse<IEnumerable<MissionField>>>(() =>
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
                var response = new RepositoryResponse<IEnumerable<MissionField>>();
                response.Succeed(string.Format("{0} items(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.Name);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<MissionField>> Find(IEnumerable<string> terms)
        {
            var resp = new RepositoryResponse<IEnumerable<MissionField>>();
            try
            {
                var items = this.All();
                resp.Entity = items
                    .Where(c => terms.Any(t => c.Name.ToLower().IndexOf(t.ToLower()) > -1))
                    .OrderBy(c => c.Name)
                    .ToList()
                ;
                resp.Succeed(string.Format("{0} {1} found.", resp.Entity.Count(), "Mission Fields"));
            }
            catch (Exception ex)
            {
                resp.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return resp;
        }

        public RepositoryResponse UpdateContactDisplayOrder(int missionFieldID, int[] roleDefinitionIDs)
        {
            var response = new RepositoryResponse();
            try
            {
                var item = DbSet
                    .Include("RoleDefinitions")
                    .SingleOrDefault(p => p.MissionFieldID == missionFieldID);

                if (item != null && item.RoleDefinitions != null)
                {
                    for (int i = 0; i < roleDefinitionIDs.Length; i++)
                    {
                        var roleDefinitionID = roleDefinitionIDs[i];
                        var roleDefinition = item.RoleDefinitions.SingleOrDefault(p => p.RoleDefinitionID == roleDefinitionID);
                        if (roleDefinition != null)
                        {
                            roleDefinition.DisplayOrder = i;
                        }
                    }
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