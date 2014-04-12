using System.Data.Entity;
using Clickfarm.Cms.Data.EntityFramework;
using KCActorsTheatre;
using KCActorsTheatre.Cms;
using KCActorsTheatre.Cms.AppTypes;
using KCActorsTheatre.Cms.ContentTypes;
using System.Data.Entity.ModelConfiguration.Conventions;
using KCActorsTheatre.Resources;
using KCActorsTheatre.Locations;
using KCActorsTheatre.Staff;
using KCActorsTheatre.Announcements;
using KCActorsTheatre.MissionStories;
using KCActorsTheatre.Tags;
using KCActorsTheatre.Organizations;
using KCActorsTheatre.Blogs;
using KCActorsTheatre.Calendar;

namespace KCActorsTheatre.Data
{
    public class KCActorsTheatreDbContext : EntityFrameworkCmsDbContext, IKCActorsTheatreDbContext
    {
        public KCActorsTheatreDbContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

        public DbSet<Post> Posts
        {
            get { return Set<Post>(); }
        }

        public DbSet<PostCategory> PostCategories
        {
            get { return Set<PostCategory>(); }
        }

        public DbSet<EventCategory> EventCategories
        {
            get { return Set<EventCategory>(); }
        }

        public DbSet<CalendarEvent> CalendarEvents
        {
            get { return Set<CalendarEvent>(); }
        }

        public DbSet<Resource> Resources
        {
            get { return Set<Resource>(); }
        }

        public DbSet<Tag> Tags
        {
            get { return Set<Tag>(); }
        }

        public DbSet<Congregation> Congregations
        {
            get { return Set<Congregation>(); }
        }

        public DbSet<CongregationContact> CongregationContacts
        {
            get { return Set<CongregationContact>(); }
        }

        public DbSet<MissionCenter> MissionCenters
        {
            get { return Set<MissionCenter>(); }
        }

        public DbSet<MissionField> MissionFields
        {
            get { return Set<MissionField>(); }
        }

        public DbSet<Organization> Organizations
        {
            get { return Set<Organization>(); }
        }

        public DbSet<StaffMember> StaffMembers
        {
            get { return Set<StaffMember>(); }
        }

        public DbSet<RoleDefinition> RoleDefinitions
        {
            get { return Set<RoleDefinition>(); }
        }

        public DbSet<Announcement> Announcements
        {
            get { return Set<Announcement>(); }
        }

        public DbSet<MissionStory> MissionStories
        {
            get { return Set<MissionStory>(); }
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // don't plurarize table names
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<KCActorsTheatreApp>().Map(m => m.Requires("AppTypeDiscriminator").HasValue("KCActorsTheatreApp"));

            var resourceDiscriminator = "ResourceDiscriminator";

            modelBuilder.Entity<PostCategory>().Map(m => m.ToTable("PostCategory", "Blog"));

            modelBuilder.Entity<Post>().Map(m => m.ToTable("Post", "Blog"));

            modelBuilder.Entity<Post>()
                .HasMany(a => a.Categories)
                .WithMany(c => c.Posts)
                .Map(m =>
                {
                    m.ToTable("PostPostCategory", "Blog");
                    m.MapLeftKey("PostID");
                    m.MapRightKey("PostCategoryID");
                })
                ;

            modelBuilder.Entity<Post>()
                .HasMany(a => a.Tags)
                .WithMany(c => c.Posts)
                .Map(m =>
                {
                    m.ToTable("PostTag", "Blog");
                    m.MapLeftKey("PostID");
                    m.MapRightKey("TagID");
                })
                ;

            modelBuilder.Entity<Resource>()
                .Map(m => m.ToTable("Resource", "COC"))
                .Map<AudioResource>(m => m.Requires(resourceDiscriminator).HasValue((int)ResourceType.Audio))
                .Map<DocumentResource>(m => m.Requires(resourceDiscriminator).HasValue((int)ResourceType.Document))
                .Map<MediaResource>(m => m.Requires(resourceDiscriminator).HasValue((int)ResourceType.Media))
                .Map<ProductResource>(m => m.Requires(resourceDiscriminator).HasValue((int)ResourceType.Product))
                .Map<PublicationResource>(m => m.Requires(resourceDiscriminator).HasValue((int)ResourceType.Publication))
                .Map<SlideshowResource>(m => m.Requires(resourceDiscriminator).HasValue((int)ResourceType.Slideshow))
                .Map<VideoResource>(m => m.Requires(resourceDiscriminator).HasValue((int)ResourceType.Video))
                .Map<PresentationResource>(m => m.Requires(resourceDiscriminator).HasValue((int)ResourceType.Presentation))
                .HasMany(p => p.Tags)
                .WithMany(p => p.Resources)
                .Map(m => 
                {
                    m.ToTable("ResourceTag", "COC");
                    m.MapLeftKey("ResourceID");
                    m.MapRightKey("TagID");
                })
                ;

            modelBuilder.Entity<Announcement>()
                .Map(m => m.ToTable("Announcement", "COC"))
                .HasMany(p => p.Tags)
                .WithMany(p => p.Announcements)
                .Map(m =>
                {
                    m.ToTable("AnnouncementTag", "COC");
                    m.MapLeftKey("AnnouncementID");
                    m.MapRightKey("TagID");
                })
                ;

            modelBuilder.Entity<MissionStory>()
                .Map(m => m.ToTable("MissionStory", "COC"))
                .HasMany(p => p.Tags)
                .WithMany(p => p.MissionStories)
                .Map(m =>
                {
                    m.ToTable("MissionStoryTag", "COC");
                    m.MapLeftKey("MissionStoryID");
                    m.MapRightKey("TagID");
                })
                ;
            
            modelBuilder.Entity<Tag>()
                .Map(m => m.ToTable("Tag", "COC"))
                ;

            modelBuilder.Entity<EventCategory>().Map(m => m.ToTable("EventCategory", "Calendar"));

            modelBuilder.Entity<CalendarEvent>()
                .Map(m => m.ToTable("CalendarEvent", "Calendar"))
                .HasMany(p => p.Tags)
                .WithMany(p => p.CalendarEvents)
                .Map(m =>
                {
                    m.ToTable("CalendarEventTag", "Calendar");
                    m.MapLeftKey("CalendarEventID");
                    m.MapRightKey("TagID");
                })
                ;


            modelBuilder.Entity<Congregation>().Map(m => m.ToTable("Congregation", "COC"));

            modelBuilder.Entity<CongregationContact>().Map(m => m.ToTable("CongregationContact", "COC"));

            modelBuilder.Entity<MissionCenter>().Map(m => m.ToTable("MissionCenter", "COC"));

            modelBuilder.Entity<MissionField>().Map(m => m.ToTable("MissionField", "COC"));

            modelBuilder.Entity<Organization>().Map(m => m.ToTable("Organization", "COC"));

            var staffMemberDiscriminator = "StaffMemberDiscriminator";

            modelBuilder.Entity<StaffMember>().Map(m => m.ToTable("StaffMember", "COC"))
                .Map<RegularStaffMember>(m => m.Requires(staffMemberDiscriminator).HasValue((int)StaffType.StaffMember))
                .Map<ConnectWidgetContact>(m => m.Requires(staffMemberDiscriminator).HasValue((int)StaffType.ConnectWidgetContact))
                ;

            modelBuilder.Entity<RoleDefinition>().Map(m => m.ToTable("RoleDefinition", "COC"));

            modelBuilder.Entity<RoleDefinition>()
                .HasRequired(m => m.StaffMember)
                .WithMany(p => p.RoleDefinitions)
                .HasForeignKey(p => p.StaffMemberID)
                ;

            #region Custom Content Types

            // callout text
            modelBuilder.Entity<CalloutTextContent>().Map(m => m.Requires("ContentType").HasValue("CalloutText"));

            // hero image
            modelBuilder.Entity<HeroImageContent>().Map(m => m.Requires("ContentType").HasValue("HeroImage"));
            modelBuilder.Entity<LandingPageHeroImageContent>().Map(m => m.Requires("ContentType").HasValue("LandingPageHeroImage"));

            // rotator image
            modelBuilder.Entity<RotatorImageContent>().Map(m => m.Requires("ContentType").HasValue("RotatorImage"));
            modelBuilder.Entity<RotatorImageContent>().Property(p => p.RotatorCaption).HasColumnName("RotatorCaption");
            modelBuilder.Entity<RotatorImageContent>().Property(p => p.RotatorOrder).HasColumnName("RotatorOrder");
            modelBuilder.Entity<RotatorImageContent>().Property(p => p.RotatorTitle).HasColumnName("RotatorTitle");

            // content module
            modelBuilder.Entity<ContentWidgetContent>().Map(m => m.Requires("ContentType").HasValue("ContentWidget"));
            modelBuilder.Entity<ContentWidgetContent>().Property(p => p.Title).HasColumnName("ContentModuleTitle");
            modelBuilder.Entity<ContentWidgetContent>().Property(p => p.Text).HasColumnName("ContentModuleText");
            modelBuilder.Entity<ContentWidgetContent>().Property(p => p.DisplayOrder).HasColumnName("ContentModuleDisplayOrder");

            // Widget content type collections
            modelBuilder.Entity<ConnectWidgetStaffMember>().Map(m => m.ToTable("ConnectWidgetStaffMember", "COC"));

            // widget content types
            modelBuilder.Entity<TaggedWidgetContent>().Map(m => m.Requires("ContentType").HasValue("TaggedWidget"))
                .HasMany(p => p.Tags)
                .WithMany(p => p.TaggedWidgets)
                .Map(m =>
                {
                    m.ToTable("TaggedWidgetTag", "COC");
                    m.MapLeftKey("ContentID");
                    m.MapRightKey("TagID");
                })
                ;

            modelBuilder.Entity<ConnectWidgetContent>().Map(m => m.Requires("ContentType").HasValue("ConnectWidget"));
            modelBuilder.Entity<ResourceWidgetContent>().Map(m => m.Requires("ContentType").HasValue("ResourceWidget"));
            modelBuilder.Entity<AnnouncementWidgetContent>().Map(m => m.Requires("ContentType").HasValue("AnnouncementWidget"));
            modelBuilder.Entity<MissionStoryWidgetContent>().Map(m => m.Requires("ContentType").HasValue("MissionStoryWidget"));
            modelBuilder.Entity<HomePageContentWidgetContent>().Map(m => m.Requires("ContentType").HasValue("HomePageContentWidget"));
            modelBuilder.Entity<HomePageMissionStoryWidgetContent>().Map(m => m.Requires("ContentType").HasValue("HomePageMissionStoryWidget"));
            modelBuilder.Entity<AlertContent>().Map(m => m.Requires("ContentType").HasValue("AlertContent"));

            #endregion

        }
    }
}