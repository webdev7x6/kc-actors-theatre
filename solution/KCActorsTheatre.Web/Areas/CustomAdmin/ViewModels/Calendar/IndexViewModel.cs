using Clickfarm.Cms.Admin.Mvc.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCActorsTheatre.Calendar;


namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Calendar
{
    public class IndexViewModel : AdminViewModel
    {
        public CalendarEvent CalendarEvent = new CalendarEvent();
        public IEnumerable<SelectListItem> EventCategoryList;
    }
}