using KCActorsTheatre.MissionStories;
using System.Collections.Generic;

namespace KCActorsTheatre.Web.ViewModels
{
    public class MissionStoriesViewModel : KCActorsTheatreViewModel
    {
        public IEnumerable<MissionStory> MissionStories { get; set; }
    }
}