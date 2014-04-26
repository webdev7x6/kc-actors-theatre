using System.Data.Objects;
using Clickfarm.Cms.Data.EntityFramework;
using KCActorsTheatre.Data.Repositories;

namespace KCActorsTheatre.Data
{
    public class KCActorsTheatreRepository : EntityFrameworkCmsRepository, IKCActorsTheatreRepository
    {
        private KCActorsTheatreDbContext context;
        private NewsArticleRepository newsArticleRepository = null;
        private ShowRepository showRepository = null;
        private PersonRepository personRepository = null;
        private SeasonRepository seasonRepository = null;
        

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

        public ShowRepository Shows
        {
            get
            {
                if (showRepository == null)
                {
                    showRepository = new ShowRepository(context, this);
                }
                return showRepository;
            }
        }

        public PersonRepository People
        {
            get
            {
                if (personRepository == null)
                {
                    personRepository = new PersonRepository(context, this);
                }
                return personRepository;
            }
        }

        public SeasonRepository Seasons
        {
            get
            {
                if (seasonRepository == null)
                {
                    seasonRepository = new SeasonRepository(context, this);
                }
                return seasonRepository;
            }
        }

        public override void RefreshAll()
        {
            base.RefreshAll();
            context.Refresh(RefreshMode.StoreWins, context.NewsArticles);
            context.Refresh(RefreshMode.StoreWins, context.Shows);
            context.Refresh(RefreshMode.StoreWins, context.People);
            context.Refresh(RefreshMode.StoreWins, context.Seasons);
        }
    }
}