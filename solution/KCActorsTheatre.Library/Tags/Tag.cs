using Clickfarm.Cms.Core;
using KCActorsTheatre.Cms.ContentTypes;
using KCActorsTheatre.Announcements;
using KCActorsTheatre.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCActorsTheatre.MissionStories;
using KCActorsTheatre.Calendar;
using KCActorsTheatre.Blogs;

namespace KCActorsTheatre.Tags
{
    public class Tag
    {
        public int TagID { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string Name { get; set; }

        public TagType TagType { get; set; }

        private HashSet<Resource> _resources = new HashSet<Resource>();
        public ICollection<Resource> Resources { get { return _resources; } }

        private HashSet<MissionStory> _missionStories = new HashSet<MissionStory>();
        public ICollection<MissionStory> MissionStories { get { return _missionStories; } }

        private HashSet<Announcement> _announcements = new HashSet<Announcement>();
        public ICollection<Announcement> Announcements { get { return _announcements; } }

        private HashSet<CalendarEvent> _calendarEvents = new HashSet<CalendarEvent>();
        public ICollection<CalendarEvent> CalendarEvents { get { return _calendarEvents; } }

        private HashSet<Post> _posts = new HashSet<Post>();
        public ICollection<Post> Posts { get { return _posts; } }

        private HashSet<TaggedWidgetContent> _taggedWidgets = new HashSet<TaggedWidgetContent>();
        public ICollection<TaggedWidgetContent> TaggedWidgets { get { return _taggedWidgets; } }
    }
}