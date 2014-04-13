using System.Data.Objects;
using Clickfarm.Cms.Data.EntityFramework;
using KCActorsTheatre.Data.Repositories;

namespace KCActorsTheatre.Data
{
    public class KCActorsTheatreRepository : EntityFrameworkCmsRepository, IKCActorsTheatreRepository
    {
        private KCActorsTheatreDbContext context;
        private NewsArticleRepository newsArticleRepository = null;

        public KCActorsTheatreRepository(KCActorsTheatreDbContext entityFrameworkCmsDbContext)
            : base(entityFrameworkCmsDbContext)
        {
            this.context = entityFrameworkCmsDbContext;
        }

        public NewsArticleRepository NewsArticles
        {
            get
            {
                if (newsArticleRepository == null)
                {
                    newsArticleRepository = new NewsArticleRepository(context, this);
                }
                return newsArticleRepository;
            }
        }

        public override void RefreshAll()
        {
            base.RefreshAll();
            context.Refresh(RefreshMode.StoreWins, context.NewsArticles);
        }
    }
}