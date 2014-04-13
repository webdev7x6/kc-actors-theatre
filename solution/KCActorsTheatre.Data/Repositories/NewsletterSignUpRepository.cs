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
    public class NewsletterSignUpRepository : KCActorsTheatreRepositoryBase<NewsletterSignUp>
    {
        public NewsletterSignUpRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository) : base(context, repository) { }

        protected override DbSet<NewsletterSignUp> DbSet
        {
            get { return dbContext.NewsletterSignUps; }
        }

        public RepositoryResponse<NewsletterSignUp> New(NewsletterSignUp item)
        {
            return CatchError<RepositoryResponse<NewsletterSignUp>>(() =>
            {
                DbSet.Add(item);
                CmsDbContext.Save();
                var response = new RepositoryResponse<NewsletterSignUp>();
                response.Succeed("The new item was created successfully.");
                response.Entity = item;
                return response;
            });
        }
    }
}