using Clickfarm.Cms.Data.Repositories;
using KCActorsTheatre.Data.Repositories;

namespace KCActorsTheatre.Data
{
    public interface IKCActorsTheatreRepository : ICmsRepository
    {
        ResourceRepository Resources { get; }
        TagRepository Tags { get; }
        CongregationRepository Congregations { get; }
        CongregationContactRepository CongregationContacts { get; }
        MissionCenterRepository MissionCenters { get; }
        MissionFieldRepository MissionFields { get; }
        StaffMemberRepository StaffMembers { get; }
        AnnouncementRepository Announcements { get; }
        MissionStoryRepository MissionStories { get; }
        OrganizationRepository Organizations { get; }
        PostRepository Posts { get; }
        PostCategoryRepository PostCategories { get; }
        EventCategoryRepository EventCategories { get; }
        CalendarEventRepository CalendarEvents { get; }
    }
}
