using Clickfarm.Cms.Core;
using KCActorsTheatre.Announcements;
using KCActorsTheatre.Web.ViewModels;
using System.Web;
using System.Web.Mvc;

namespace KCActorsTheatre.Web.Controllers
{
    public class NewsController : KCActorsTheatreController
    {
        public NewsController(ICmsContext context, HttpContextBase httpContext) : base(context, httpContext) { }

        public ActionResult Announcements()
        {
            var vm = new AnnouncementsViewModel();
            vm.AnnouncementType = AnnouncementType.Announcement;
            InitializeViewModel(vm);
            vm.Announcements = repository.Announcements.GetForLandingPage(vm.AnnouncementType).Entity;
            return View(vm);
        }

        public ActionResult OfficialAnnouncements()
        {
            var vm = new AnnouncementsViewModel();
            vm.AnnouncementType = AnnouncementType.Official;
            InitializeViewModel(vm);
            vm.Announcements = repository.Announcements.GetForLandingPage(vm.AnnouncementType).Entity;
            return View("Announcements", vm);
        }

        public ActionResult MissionStories()
        {
            var vm = new MissionStoriesViewModel();
            InitializeViewModel(vm);
            vm.MissionStories = repository.MissionStories.GetForLandingPage().Entity;
            return View(vm);
        }

        public ActionResult MissionStory(int? id)
        {
            var vm = new MissionStoryViewModel();
            InitializeViewModel(vm);
            if (id != null)
            {
                var repoResponse = repository.MissionStories.GetForDetailPage(id.Value);
                if (repoResponse.Succeeded)
	            {
                    vm.MissionStory = repoResponse.Entity;
	            }
            }
            return View(vm);
        }

    }
}