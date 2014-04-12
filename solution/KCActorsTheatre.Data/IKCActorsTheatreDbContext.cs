using System.Data.Entity;
using Clickfarm.Cms.Data;
using KCActorsTheatre.Resources;
using KCActorsTheatre.Locations;
using KCActorsTheatre.Staff;
using KCActorsTheatre.Announcements;
using KCActorsTheatre.Tags;
using KCActorsTheatre.MissionStories;
using KCActorsTheatre.Organizations;
using KCActorsTheatre.Blogs;
using KCActorsTheatre.Calendar;

namespace KCActorsTheatre.Data
{
    public interface IKCActorsTheatreDbContext : ICmsDbContext
    {
        DbSet<Tag> Tags { get; }
        DbSet<Resource> Resources { get; }
        DbSet<Congregation> Congregations { get; }
        DbSet<CongregationContact> CongregationContacts { get; }

        DbSet<MissionCenter> MissionCenters { get; }
        DbSet<MissionField> MissionFields { get; }
        DbSet<Organization> Organizations { get; }

        DbSet<StaffMember> StaffMembers { get; }
        DbSet<RoleDefinition> RoleDefinitions { get; }

        DbSet<Announcement> Announcements { get; }
        DbSet<MissionStory> MissionStories { get; }

        DbSet<Post> Posts { get; }

        DbSet<PostCategory> PostCategories { get; }
        DbSet<EventCategory> EventCategories { get; }

        DbSet<CalendarEvent> CalendarEvents { get; }
    }
}
