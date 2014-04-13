using System.Data.Entity;
using Clickfarm.Cms.Data.EntityFramework;
using KCActorsTheatre.Cms;
using KCActorsTheatre.Blog;
using KCActorsTheatre.Library.AppTypes;
using KCActorsTheatre.Calendar;
using KCActorsTheatre.Cms.ContentTypes;

namespace KCActorsTheatre.Data
{
    public class KCActorsTheatreDbContext : EntityFrameworkCmsDbContext, IKCActorsTheatreDbContext
    {
        public KCActorsTheatreDbContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

        public DbSet<Event> Events
        {
            get { return Set<Event>(); }
        }

        public DbSet<Comment> Comments
        {
            get { return Set<Comment>(); }
        }

        public DbSet<Post> Posts
        {
            get { return Set<Post>(); }
        }

        public DbSet<Author> Authors
        {
            get { return Set<Author>(); }
        }

        public DbSet<NewsletterSignUp> NewsletterSignUps
        {
            get { return Set<NewsletterSignUp>(); }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<KCActorsTheatreApp>().Map(m => m.Requires("AppTypeDiscriminator").HasValue("KCActorsTheatreApp"));

            //custom entities

            modelBuilder.Entity<Event>().Map(m => m.ToTable("Event", "Calendar"));

            modelBuilder.Entity<Comment>().Map(m => m.ToTable("Comment", "Blog"));

            modelBuilder.Entity<Post>().Map(m => m.ToTable("Post", "Blog"));

            modelBuilder.Entity<Author>().Map(m => m.ToTable("Author", "Blog"));

            modelBuilder.Entity<NewsletterSignUp>().Map(m => m.ToTable("NewsletterSignUp", "HN"));

            //custom content types
            modelBuilder.Entity<CalloutTextContent>().Map(m => m.Requires("ContentType").HasValue("CalloutText"));
            modelBuilder.Entity<HeroImageContent>().Map(m => m.Requires("ContentType").HasValue("HeroImage"));
            modelBuilder.Entity<RotatorImageContent>().Map(m => m.Requires("ContentType").HasValue("RotatorImage"));
            modelBuilder.Entity<RotatorVideoContent>().Map(m => m.Requires("ContentType").HasValue("RotatorVideo"));

        }
    }
}