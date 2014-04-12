using Clickfarm.Cms.Core;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
using System.Web;
using KCActorsTheatre.Calendar;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.EventCategories
{
    public class EditViewModel
    {
        public EventCategory EventCategory { get; set; }

        public bool HasEventCategory
        {
            get { return EventCategory != null; }
        }

        public IUtcDateConverter DateConverter { get; set; }
    }
}