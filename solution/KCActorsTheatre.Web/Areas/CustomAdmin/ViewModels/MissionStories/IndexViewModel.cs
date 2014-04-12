using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KCActorsTheatre.MissionStories;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.MissionStories
{
    public class IndexViewModel : AdminViewModel
    {
        public MissionStory MissionStory = new MissionStory();
    }
}