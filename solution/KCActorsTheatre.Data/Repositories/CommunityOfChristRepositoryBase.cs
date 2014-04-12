using System;
using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using Clickfarm.Cms.Data.Repositories;

namespace KCActorsTheatre.Data.Repositories
{
    public abstract class KCActorsTheatreRepositoryBase<T> : RepositoryBase<T>, IEntityRepository<T>
         where T : class
    {
        protected readonly IKCActorsTheatreDbContext dbContext;

        public KCActorsTheatreRepositoryBase(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository)
            : base(context, repository)
        {
            dbContext = context;
        }

        protected TResponse CatchError<TResponse>(Func<TResponse> action)
            where TResponse : RepositoryResponse, new()
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                var resp = new TResponse();
                resp.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
                return resp;
            }
        }
    }
}
