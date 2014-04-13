using System.Data.Entity;
using Clickfarm.Cms.Data.EntityFramework;
using KCActorsTheatre.Cms;
using KCActorsTheatre.Library.AppTypes;
using KCActorsTheatre.News;
using KCActorsTheatre.Cms.ContentTypes;
using KCActorsTheatre.Show;

namespace KCActorsTheatre.Data
{
    public class KCActorsTheatreDbContext : EntityFrameworkCmsDbContext, IKCActorsTheatreDbContext
    {
        public KCActorsTheatreDbContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

        public DbSet<Article> NewsArticles
        {
            get { return Set<Article>(); }
        }

        public DbSet<ShowInfo> Shows
        {
            get { return Set<ShowInfo>(); }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<KCActorsTheatreApp>().Map(m => m.Requires("AppTypeDiscriminator").HasValue("KCActorsTheatreApp"));

            //custom entities

            modelBuilder.Entity<Article>().Map(m => m.ToTable("Article", "KCAT"));
            modelBuilder.Entity<ShowInfo>().Map(m => m.ToTable("Show", "KCAT"));

            //custom content types
            modelBuilder.Entity<CalloutTextContent>().Map(m => m.Requires("ContentType").HasValue("CalloutText"));
            modelBuilder.Entity<HeroImageContent>().Map(m => m.Requires("ContentType").HasValue("HeroImage"));
            modelBuilder.Entity<RotatorImageContent>().Map(m => m.Requires("ContentType").HasValue("RotatorImage"));
            modelBuilder.Entity<RotatorVideoContent>().Map(m => m.Requires("ContentType").HasValue("RotatorVideo"));

        }
    }
}