using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using KCActorsTheatre.Blog;

namespace KCActorsTheatre.Data.Repositories
{
    public class CommentRepository : KCActorsTheatreRepositoryBase<Comment>
    {
        public CommentRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository) : base(context, repository) { }

        protected override DbSet<Comment> DbSet
        {
            get { return dbContext.Comments; }
        }

        public RepositoryResponse<Comment> New(Comment item)
        {
            return CatchError<RepositoryResponse<Comment>>(() =>
            {
                DbSet.Add(item);
                CmsDbContext.Save();
                var response = new RepositoryResponse<Comment>();
                response.Succeed("The new item was created successfully.");
                response.Entity = item;
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Comment>> FindForDisplay(IEnumerable<string> terms)
        {
            return CatchError<RepositoryResponse<IEnumerable<Comment>>>(() =>
            {
                var items = All(new string[] { "Post" }, enableTracking: false);
                if (terms != null && terms.Count() > 0)
                {
                    items = items.Where(a =>
                        terms.Any(t =>
                            a.Text != null && a.Text.IndexOf(t, StringComparison.CurrentCultureIgnoreCase) >= 0
                        )
                    );
                }
                var response = new RepositoryResponse<IEnumerable<Comment>>();
                response.Succeed(string.Format("{0} items(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.IsApproved).ThenBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Comment>> GetAll()
        {
            return CatchError<RepositoryResponse<IEnumerable<Comment>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<Comment>>();
                var items = All(new string[] { "Post" }, enableTracking: false);
                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.IsApproved).ThenBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<Comment> GetSingle(int id)
        {
            return CatchError<RepositoryResponse<Comment>>(() =>
            {
                var item = Single(a => a.CommentID == id, new string[] { "Post" }, enableTracking: true);
                var response = new RepositoryResponse<Comment>();
                if (item != null)
                {
                    response.Succeed(string.Format("Item with ID {0} found.", item.CommentID));
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
                    CmsDbContext.ChangeState<Comment>(EntityState.Deleted, item);
                    CmsDbContext.Save();
                    response.Succeed(string.Format("Item with ID {0} deleted.", id));
                }
                else
                {
                    response.Fail(string.Format("Item with ID {0} not found.", id));
                }
                return response;
            });
        }

        public RepositoryResponse Modify(int[] idArray, string action)
        {
            return CatchError<RepositoryResponse>(() =>
            {
                var response = new RepositoryResponse();
                foreach (var id in idArray)
                {
                    var item = Single(p => p.CommentID == id, new string[] { "Post" });
                    if (item != null)
                    {
                        switch (action)
                        {
                            case "approve":
                                item.IsApproved = true;
                                item.DateApproved = DateTime.UtcNow;
                                CmsDbContext.ChangeState<Comment>(EntityState.Modified, item);
                                break;
                            case "unapprove":
                                item.IsApproved = false;
                                item.DateApproved = null;
                                CmsDbContext.ChangeState<Comment>(EntityState.Modified, item);
                                break;
                            case "delete":
                                CmsDbContext.ChangeState<Comment>(EntityState.Deleted, item);
                                break;
                        }
                    }
                }
                CmsDbContext.Save();
                response.Succeed("Items updated.");
                return response;
            });
        }
    }
}