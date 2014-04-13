using System.Data.Entity;
using Clickfarm.Cms.Data.EntityFramework;
using KCActorsTheatre.Cms;
using KCActorsTheatre.Library.AppTypes;
using KCActorsTheatre.News;
using KCActorsTheatre.Cms.ContentTypes;

namespace KCActorsTheatre.Data
{
    public class KCActorsTheatreDbContext : EntityFrameworkCmsDbContext, IKCActorsTheatreDbContext
    {
        public KCActorsTheatreDbContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

        public DbSet<Article> NewsArticles
        {
            get { return Set<Article>(); }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<KCActorsTheatreApp>().Map(m => m.Requires("AppTypeDiscriminator").HasValue("KCActorsTheatreApp"));

            //custom entities

            modelBuilder.Entity<Article>().Map(m => m.ToTable("Article", "KCAT"));

            //custom content types
            modelBuilder.Entity<CalloutTextContent>().Map(m => m.Requires("ContentType").HasValue("CalloutText"));
            modelBuilder.Entity<HeroImageContent>().Map(m => m.Requires("ContentType").HasValue("HeroImage"));
            modelBuilder.Entity<RotatorImageContent>().Map(m => m.Requires("ContentType").HasValue("RotatorImage"));
            modelBuilder.Entity<RotatorVideoContent>().Map(m => m.Requires("ContentType").HasValue("RotatorVideo"));

        }
    }
}