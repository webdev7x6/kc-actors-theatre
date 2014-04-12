using Clickfarm.Cms.Core;
using KCActorsTheatre.Locations;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
using System.Web;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.MissionCenters
{
    public class EditViewModel
    {
        public MissionCenter MissionCenter { get; set; }

        public bool HasMissionCenter
        {
            get { return MissionCenter != null; }
        }

        public IUtcDateConverter DateConverter { get; set; }

        public ImageContentProperties ContentProperties_ThumbnailImage { get; set; }

        public FileContentProperties ContentProperties_ThumbnailImageFile { get; set; }

        public FileContentProperties ContentProperties_ResourceFile { get; set; }
    }
}