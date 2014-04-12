using Clickfarm.Cms.Data.EntityFramework;
using KCActorsTheatre.Data.Repositories;
using System.Data.Entity.Core.Objects;

namespace KCActorsTheatre.Data
{
    public class KCActorsTheatreRepository : EntityFrameworkCmsRepository, IKCActorsTheatreRepository
    {
        private KCActorsTheatreDbContext context;
        private ResourceRepository resourceRepo = null;
        private TagRepository tagRepo = null;
        private CongregationRepository congregationRepo = null;
        private CongregationContactRepository congregationContactRepo = null;
        private MissionCenterRepository missionCenterRepo = null;
        private MissionFieldRepository missionFieldRepo = null;
        private StaffMemberRepository staffMemberRepo = null;
        private AnnouncementRepository announcementRepo = null;
        private MissionStoryRepository missionStoryRepo = null;
        private OrganizationRepository organizationRepo = null;
        private PostRepository postRepository = null;
        private PostCategoryRepository postCategoryRepository = null;
        private EventCategoryRepository eventCategoryRepository = null;
        private CalendarEventRepository calendarEventRepository = null;

        public KCActorsTheatreRepository(KCActorsTheatreDbContext entityFrameworkCmsDbContext)
            : base(entityFrameworkCmsDbContext)
        {
            this.context = entityFrameworkCmsDbContext;
        }

        public ResourceRepository Resources
        {
            get
            {
                if (resourceRepo == null)
                {
                    resourceRepo = new ResourceRepository(context, this);
                }
                return resourceRepo;
            }
        }

        public TagRepository Tags
        {
            get
            {
                if (tagRepo == null)
                {
                    tagRepo = new TagRepository(context, this);
                }
                return tagRepo;
            }
        }

        public CongregationRepository Congregations
        {
            get
            {
                if (congregationRepo == null)
                {
                    congregationRepo = new CongregationRepository(context, this);
                }
                return congregationRepo;
            }
        }

        public CongregationContactRepository CongregationContacts
        {
            get
            {
                if (congregationContactRepo == null)
                {
                    congregationContactRepo = new CongregationContactRepository(context, this);
                }
                return congregationContactRepo;
            }
        }

        public MissionCenterRepository MissionCenters
        {
            get
            {
                if (missionCenterRepo == null)
                {
                    missionCenterRepo = new MissionCenterRepository(context, this);
                }
                return missionCenterRepo;
            }
        }

        public MissionFieldRepository MissionFields
        {
            get
            {
                if (missionFieldRepo == null)
                {
                    missionFieldRepo = new MissionFieldRepository(context, this);
                }
                return missionFieldRepo;
            }
        }

        public StaffMemberRepository StaffMembers
        {
            get
            {
                if (staffMemberRepo == null)
                {
                    staffMemberRepo = new StaffMemberRepository(context, this);
                }
                return staffMemberRepo;
            }
        }

        public AnnouncementRepository Announcements
        {
            get
            {
                if (announcementRepo == null)
                {
                    announcementRepo = new AnnouncementRepository(context, this);
                }
                return announcementRepo;
            }
        }

        public MissionStoryRepository MissionStories
        {
            get
            {
                if (missionStoryRepo == null)
                {
                    missionStoryRepo = new MissionStoryRepository(context, this);
                }
                return missionStoryRepo;
            }
        }

        public OrganizationRepository Organizations
        {
            get
            {
                if (organizationRepo == null)
                {
                    organizationRepo = new OrganizationRepository(context, this);
                }
                return organizationRepo;
            }
        }

        public PostRepository Posts
        {
            get { return postRepository ?? (postRepository = new PostRepository(context, this)); }
        }

        public PostCategoryRepository PostCategories
        {
            get { return postCategoryRepository ?? (postCategoryRepository = new PostCategoryRepository(context, this)); }
        }

        public EventCategoryRepository EventCategories
        {
            get { return eventCategoryRepository ?? (eventCategoryRepository = new EventCategoryRepository(context, this)); }
        }

        public CalendarEventRepository CalendarEvents
        {
            get { return calendarEventRepository ?? (calendarEventRepository = new CalendarEventRepository(context, this)); }
        }

        public override void RefreshAll()
        {
            base.RefreshAll();
            context.Refresh(RefreshMode.StoreWins, context.Resources);
            context.Refresh(RefreshMode.StoreWins, context.Tags);
            context.Refresh(RefreshMode.StoreWins, context.Congregations);
            context.Refresh(RefreshMode.StoreWins, context.CongregationContacts);
            context.Refresh(RefreshMode.StoreWins, context.MissionCenters);
            context.Refresh(RefreshMode.StoreWins, context.MissionFields);
            context.Refresh(RefreshMode.StoreWins, context.StaffMembers);
            context.Refresh(RefreshMode.StoreWins, context.Announcements);
            context.Refresh(RefreshMode.StoreWins, context.MissionStories);
            context.Refresh(RefreshMode.StoreWins, context.Organizations);
            context.Refresh(RefreshMode.StoreWins, context.CalendarEvents);
            //context.Refresh(RefreshMode.StoreWins, context.Events);
            //context.Refresh(RefreshMode.StoreWins, context.Posts);
            //context.Refresh(RefreshMode.StoreWins, context.Comments);
            //context.Refresh(RefreshMode.StoreWins, context.Authors);
        }
    }
}