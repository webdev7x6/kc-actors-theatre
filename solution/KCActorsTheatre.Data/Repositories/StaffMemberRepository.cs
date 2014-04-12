using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using KCActorsTheatre;
using KCActorsTheatre.Cms.ContentTypes;
using KCActorsTheatre.Staff;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCActorsTheatre.Data.Repositories
{
    public class StaffMemberRepository : KCActorsTheatreRepositoryBase<StaffMember>
    {
        private IKCActorsTheatreRepository _repository;
        private IKCActorsTheatreDbContext _context;
        public StaffMemberRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository)
            : base(context, repository)
        {   
            _context = context;
            _repository = repository;
        }

        protected override DbSet<StaffMember> DbSet
        {
            get { return dbContext.StaffMembers; }
        }

        public RepositoryResponse<StaffMember> New(StaffMember item)
        {
            return CatchError<RepositoryResponse<StaffMember>>(() =>
            {
                DbSet.Add(item);
                CmsDbContext.Save();
                var response = new RepositoryResponse<StaffMember>();
                response.Succeed("The new item was created successfully.");
                response.Entity = item;
                return response;
            });
        }

        public RepositoryResponse<RoleDefinition> NewRoleDefinition(RoleDefinition roleDefinition)
        {
            return CatchError<RepositoryResponse<RoleDefinition>>(() =>
            {
                var response = new RepositoryResponse<RoleDefinition>();
                CmsDbContext.ChangeState<RoleDefinition>(EntityState.Added, roleDefinition);
                CmsDbContext.Save();
                response.Succeed("The new role definition was created successfully.");
                response.Entity = roleDefinition;
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<StaffMember>> FindForDisplay(IEnumerable<string> terms)
        {
            return CatchError<RepositoryResponse<IEnumerable<StaffMember>>>(() =>
            {
                var items = All(new[] { "RoleDefinitions" }, enableTracking: false)
                ;

                if (terms != null && terms.Count() > 0)
                {
                    items = items.Where(a =>
                        terms.Any(t =>
                            a.SortName != null &&
                            (a.LastName.IndexOf(t, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                            a.FirstName.IndexOf(t, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        )
                    );
                }
                var response = new RepositoryResponse<IEnumerable<StaffMember>>();
                response.Succeed(string.Format("{0} items(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<StaffMember>> FindByTypeForDisplay(IEnumerable<string> terms, StaffType staffType)
        {
            return CatchError<RepositoryResponse<IEnumerable<StaffMember>>>(() =>
            {
                var items = All(new[] { "RoleDefinitions" }, enableTracking: false)
                    .Where(p => p.StaffType == staffType)
                ;

                if (terms != null && terms.Count() > 0)
                {
                    items = items.Where(a =>
                        terms.Any(t =>
                            a.SortName != null && 
                            (a.LastName.IndexOf(t, StringComparison.CurrentCultureIgnoreCase) >= 0 || 
                            a.FirstName.IndexOf(t, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        )
                    );
                }
                var response = new RepositoryResponse<IEnumerable<StaffMember>>();
                response.Succeed(string.Format("{0} items(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<StaffMember>> GetAll()
        {
            return CatchError<RepositoryResponse<IEnumerable<StaffMember>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<StaffMember>>();

                var items = DbSet
                    .Include("RoleDefinitions")
                    .Include("RoleDefinitions.MissionField")
                    .Include("RoleDefinitions.MissionCenter")
                    .AsNoTracking()
                    ;

                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.SortName);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<StaffMember>> GetAllByType(StaffType staffType)
        {
            return CatchError<RepositoryResponse<IEnumerable<StaffMember>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<StaffMember>>();

                var items = DbSet
                    .Include("RoleDefinitions")
                    .Include("RoleDefinitions.MissionField")
                    .Include("RoleDefinitions.MissionCenter")
                    .Where(p => p.StaffType == staffType)
                    .AsNoTracking()
                    ;
                    
                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.SortName);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<string>> GetStaffMemberTitles()
        {
            return CatchError<RepositoryResponse<IEnumerable<string>>>(() =>
            {
            var response = new RepositoryResponse<IEnumerable<string>>();

                var staffMembers = DbSet
                    .Include("RoleDefinitions")
                    .AsNoTracking()
                    ;

                var titles = staffMembers.SelectMany(sm => sm.RoleDefinitions.Select(rd => rd.Title)).Distinct().ToList();

                response.Succeed(string.Format("{0} item(s) found.", titles.Count));
                response.Entity = titles;
                return response;
            });
        }


        public RepositoryResponse<StaffMember> GetSingle(int id)
        {
            return CatchError<RepositoryResponse<StaffMember>>(() =>
            {
                var response = new RepositoryResponse<StaffMember>();

                var item = DbSet
                    .Include("RoleDefinitions")
                    .Include("RoleDefinitions.MissionField")
                    .Include("RoleDefinitions.MissionCenter")
                    .FirstOrDefault(p => p.StaffMemberID == id)
                    ;


                if (item != null)
                {
                    response.Succeed(string.Format("Item with ID {0} found.", item.StaffMemberID));
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
                    .SingleOrDefault(p => p.StaffMemberID == id);

                var response = new RepositoryResponse();
                if (item != null)
                {
                    // delete RoleDefinitions
                    var roleDefinitions = item.RoleDefinitions.ToList();
                    roleDefinitions.ForEach(rd => CmsDbContext.ChangeState<RoleDefinition>(EntityState.Deleted, rd));

                    CmsDbContext.ChangeState<StaffMember>(EntityState.Deleted, item);
                    CmsDbContext.Save();
                    response.Succeed(string.Format("StaffMember with ID {0} deleted.", id));
                }
                else
                {
                    response.Fail(string.Format("StaffMember with ID {0} not found.", id));
                }
                return response;
            });
        }

        public RepositoryResponse DeleteRole(int memberID, int roleID)
        {
            return CatchError<RepositoryResponse>(() =>
            {
                var response = new RepositoryResponse();

                var item = DbSet
                    .Include("RoleDefinitions")
                    .Include("RoleDefinitions.MissionField")
                    .Include("RoleDefinitions.MissionCenter")
                    .SingleOrDefault(p => p.StaffMemberID == memberID);

                if (item != null)
                {
                    var role = item.RoleDefinitions.SingleOrDefault(p => p.RoleDefinitionID == roleID);

                    if (role != null)
                    {
                        CmsDbContext.ChangeState<RoleDefinition>(EntityState.Deleted, role);
                        CmsDbContext.Save();
                    }

                    response.Succeed(string.Format("Role definition with ID {0} deleted.", roleID));
                }
                else
                {
                    response.Fail(string.Format("Role definition with ID {0} not found.", roleID));
                }
                return response;
            });
        }

        #region CMS Widget Tagging

        public RepositoryResponse<IEnumerable<StaffMember>> GetWidgetStaff(int contentID)
        {
            //return CatchError<RepositoryResponse<IEnumerable<StaffMember>>>(() =>
            //{
                var response = new RepositoryResponse<IEnumerable<StaffMember>>();

                var content = CmsDbContext.Content
                    .OfType<ConnectWidgetContent>()
                    .Include("ConnectWidgetStaffMembers")
                    .Include("ConnectWidgetStaffMembers.StaffMember")
                    .SingleOrDefault(p => p.ContentID == contentID)
                    ;

                if (content != null)
                {
                    List<StaffMember> staffMembers = new List<StaffMember>();

                    content.ConnectWidgetStaffMembers
                        .OrderBy(p => p.DisplayOrder)
                        .ToList()
                        .ForEach(item => staffMembers.Add(item.StaffMember))
                        ;

                    response.Succeed(string.Format("{0} staff member(s) found.", staffMembers.Count));
                    response.Entity = staffMembers;
                }
                else
                {
                    response.Fail("No staff members found");
                }
                return response;
            //});
        }

        /// <summary>
        /// Adds a staff member to a connect widget
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tagID"></param>
        /// <returns></returns>
        public RepositoryResponse<StaffMember> AddWidgetStaff(int contentID, int staffMemberID)
        {
            var response = new RepositoryResponse<StaffMember>();
            try
            {
                var content = CmsDbContext.Content
                    .OfType<ConnectWidgetContent>()
                    .Include("ConnectWidgetStaffMembers")
                    .Include("ConnectWidgetStaffMembers.StaffMember")
                    .SingleOrDefault(p => p.ContentID == contentID)
                    ;

                var staffMember = DbSet
                    .SingleOrDefault(p => p.StaffMemberID == staffMemberID)
                    ;

                if (content != null && staffMember != null)
                {
                    content.ConnectWidgetStaffMembers.Add(
                        new ConnectWidgetStaffMember() {
                            ContentID = contentID,
                            StaffMemberID = staffMemberID,
                            DisplayOrder = 0,
                        }
                    );

                    CmsDbContext.Save();
                    response.Entity = staffMember;
                    response.Succeed(string.Format("Staff Member with ID {0} added to Connect Widget {1}.", staffMemberID, content.Description));
                } 
                else
                {
                    response.Fail(string.Format("ConnectWidgetContent {0} not found.", contentID));
                }
            }
            catch (Exception ex)
            {
                response.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return response;
        }

        /// <summary>
        /// removes a staff member from a connect widget
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tagID"></param>
        /// <returns></returns>
        public RepositoryResponse<StaffMember> RemoveWidgetStaff(int contentID, int staffMemberID)
        {
            var response = new RepositoryResponse<StaffMember>();
            try
            {
                var content = CmsDbContext.Content
                    .OfType<ConnectWidgetContent>()
                    .Include("ConnectWidgetStaffMembers")
                    .Include("ConnectWidgetStaffMembers.StaffMember")
                    .SingleOrDefault(p => p.ContentID == contentID)
                    ;

                var staffMember = DbSet
                    .SingleOrDefault(p => p.StaffMemberID == staffMemberID)
                    ;

                if (content != null && content.ConnectWidgetStaffMembers != null && staffMember != null)
                {
                    content.ConnectWidgetStaffMembers.RemoveAll(p => p.StaffMember == staffMember);
                    CmsDbContext.Save();

                    response.Entity = staffMember;
                    response.Succeed(string.Format("Staff Member {0} removed from Connect Widget {1}.", staffMember.SortName, content.Description));
                } 
                else
                {
                    response.Fail(string.Format("ConnectWidgetContent {0} not found.", contentID));
                }
            }
            catch (Exception ex)
            {
                response.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return response;
        }

        public RepositoryResponse UpdateStaffMembersDisplayOrder(int contentID, int[] staffMemberIDs)
        {
            var response = new RepositoryResponse();
            try
            {
                var content = CmsDbContext.Content
                    .OfType<ConnectWidgetContent>()
                    .Include(p => p.ConnectWidgetStaffMembers)
                    .SingleOrDefault(p => p.ContentID == contentID)
                    ;

                for (int i = 0; i < staffMemberIDs.Length; i++)
                {
                    var staffMemberID = staffMemberIDs[i];
                    var connectWidgetStaffMember = content.ConnectWidgetStaffMembers.FirstOrDefault(p => p.StaffMemberID == staffMemberID);
                    if (connectWidgetStaffMember != null)
                        connectWidgetStaffMember.DisplayOrder = i;
                }

                CmsDbContext.Save();

                response.Succeed("staff members display order successfully updated");
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
