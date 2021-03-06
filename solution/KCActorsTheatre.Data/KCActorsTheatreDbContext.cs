﻿using System.Data.Entity;
using Clickfarm.Cms.Data.EntityFramework;
using KCActorsTheatre.Cms;
using KCActorsTheatre.Contract;
using KCActorsTheatre.Library.AppTypes;
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

        public DbSet<ShowInfo> Shows
        {
            get { return Set<ShowInfo>(); }
        }

        public DbSet<Person> People
        {
            get { return Set<Person>(); }
        }

        public DbSet<RoleDefinition> RoleDefinitions
        {
            get { return Set<RoleDefinition>(); }
        }

        public DbSet<ShowImage> Images
        {
            get { return Set<ShowImage>(); }
        }

        public DbSet<ShowVideo> Videos
        {
            get { return Set<ShowVideo>(); }
        }
        
        public DbSet<SeasonInfo> Seasons
        {
            get { return Set<SeasonInfo>(); }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<KCActorsTheatreApp>().Map(m => m.Requires("AppTypeDiscriminator").HasValue("KCActorsTheatreApp"));

            //custom entities

            modelBuilder.Entity<Article>().Map(m => m.ToTable("Article", "KCAT"));

            modelBuilder.Entity<ShowInfo>()
                .Map(m => m.ToTable("Show", "KCAT"))
                //.HasMany(p => p.People)
                //.WithMany(p => p.Shows)
                //.Map(m =>
                //{
                //    m.ToTable("ShowPerson", "KCAT");
                //    m.MapLeftKey("ShowID");
                //    m.MapRightKey("PersonID");
                //})
                ;

            modelBuilder.Entity<Person>().Map(m => m.ToTable("Person", "KCAT"));

            modelBuilder.Entity<RoleDefinition>().Map(m => m.ToTable("RoleDefinition", "KCAT"));

            modelBuilder.Entity<RoleDefinition>()
                .HasRequired(m => m.Person)
                .WithMany(p => p.RoleDefinitions)
                .HasForeignKey(p => p.PersonID)
                ;

            modelBuilder.Entity<ShowImage>().Map(m => m.ToTable("ShowImage", "KCAT"));
            modelBuilder.Entity<ShowVideo>().Map(m => m.ToTable("ShowVideo", "KCAT"));

            modelBuilder.Entity<SeasonInfo>().Map(m => m.ToTable("Season", "KCAT"));

            //custom content types
            modelBuilder.Entity<CalloutTextContent>().Map(m => m.Requires("ContentType").HasValue("CalloutText"));
            modelBuilder.Entity<HeroImageContent>().Map(m => m.Requires("ContentType").HasValue("HeroImage"));
            modelBuilder.Entity<RotatorImageContent>().Map(m => m.Requires("ContentType").HasValue("RotatorImage"));
            modelBuilder.Entity<RotatorVideoContent>().Map(m => m.Requires("ContentType").HasValue("RotatorVideo"));

        }
    }
}