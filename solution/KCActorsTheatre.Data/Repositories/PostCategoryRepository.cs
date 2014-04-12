using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using Clickfarm.Cms.Data.Repositories;
using KCActorsTheatre.Blogs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace KCActorsTheatre.Data.Repositories
{
    public class PostCategoryRepository : KCActorsTheatreRepositoryBase<PostCategory>
    {
        private IKCActorsTheatreDbContext context;

        public PostCategoryRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository)
            : base(context, repository)
        {
            this.context = context;
        }

        protected override DbSet<PostCategory> DbSet
        {
            get { return context.PostCategories; }
        }

        public RepositoryResponse<PostCategory> New(PostCategory item)
        {
            return CatchError<RepositoryResponse<PostCategory>>(() =>
            {
                DbSet.Add(item);
                CmsDbContext.Save();

                var response = new RepositoryResponse<PostCategory>();

                var returnItem = DbSet
                    .SingleOrDefault(p => p.PostCategoryID == item.PostCategoryID);

                response.Succeed("The new item was created successfully.");
                response.Entity = returnItem;
                return response;
            });
        }
        public RepositoryResponse<IEnumerable<PostCategory>> Find(IEnumerable<string> terms)
        {
            var resp = new RepositoryResponse<IEnumerable<PostCategory>>();
            try
            {
                var catgories = this.All();
                resp.Entity = catgories
                    .Where(c => terms.Any(t => c.Name.ToLower().IndexOf(t.ToLower()) > -1))
                    .OrderBy(c => c.Name)
                    .ToList()
                ;
                resp.Succeed(string.Format("{0} {1} found.", resp.Entity.Count(), "Categories"));
            }
            catch (Exception ex)
            {
                resp.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return resp;
        }


        public RepositoryResponse<IEnumerable<PostCategory>> FindForDisplay(IEnumerable<string> terms)
        {
            RepositoryResponse<IEnumerable<PostCategory>> response = new RepositoryResponse<IEnumerable<PostCategory>>();
            try
            {
                IEnumerable<PostCategory> categories = All(null, false);
                if (terms != null && terms.Count() > 0)
                {
                    categories = categories.Where(a =>
                        terms.Any(t =>
                            a.Name != null && a.Name.IndexOf(t, StringComparison.CurrentCultureIgnoreCase) >= 0
                        )
                    );
                }
                response.Succeed(string.Format("{0} category(s) found.", categories.Count()));
                response.Entity = categories.OrderBy(a => a.Name);
            }
            catch (Exception ex)
            {
                response.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return response;
        }

        public RepositoryResponse<IEnumerable<PostCategory>> GetAll()
        {
            RepositoryResponse<IEnumerable<PostCategory>> response = new RepositoryResponse<IEnumerable<PostCategory>>();
            try
            {
                IEnumerable<PostCategory> categories = All(null, false);
                response.Succeed(string.Format("{0} categories(s) found.", categories.Count()));
                response.Entity = categories.OrderBy(a => a.Name);
            }
            catch (Exception ex)
            {
                response.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return response;
        }

        public RepositoryResponse<PostCategory> GetSingle(int articleCategoryID)
        {
            RepositoryResponse<PostCategory> response = new RepositoryResponse<PostCategory>();
            try
            {
                PostCategory category = Single(a => a.PostCategoryID == articleCategoryID);
                if (category != null)
                {
                    response.Succeed(string.Format("Category with ID {0} found.", category.PostCategoryID));
                    response.Entity = category;
                }
                else
                {
                    response.Fail(string.Format("Category with ID {0} not found.", articleCategoryID));
                }
            }
            catch (Exception ex)
            {
                response.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return response;
        }

        public RepositoryResponse Delete(int categoryID)
        {
            RepositoryResponse response = new RepositoryResponse();
            try
            {
                PostCategory category = Find(categoryID);
                if (category != null)
                {
                    CmsDbContext.ChangeState<PostCategory>(EntityState.Deleted, category);
                    CmsDbContext.Save();
                    response.Succeed(string.Format("Category with ID {0} deleted.", categoryID));
                }
                else
                {
                    response.Fail(string.Format("Category with ID {0} not found.", categoryID));
                }
            }
            catch (Exception ex)
            {
                response.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return response;
        }
    }
}
