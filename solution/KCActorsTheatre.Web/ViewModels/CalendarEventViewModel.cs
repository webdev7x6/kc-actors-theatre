using KCActorsTheatre.Calendar;
using System.Collections.Generic;

namespace KCActorsTheatre.Web.ViewModels
{
    public class CalendarEventViewModel : KCActorsTheatreViewModel
    {
        public Event Event { get; set; }

        public bool HasEvent
        {
            get { return Event != null; }
        }
    }
}