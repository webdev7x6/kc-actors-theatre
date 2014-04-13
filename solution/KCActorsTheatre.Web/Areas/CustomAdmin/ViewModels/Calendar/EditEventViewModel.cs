using Clickfarm.Cms.Core;
using KCActorsTheatre.Calendar;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Calendar
{
    public class EditEventViewModel
    {
        public Event Event { get; set; }

        public bool HasEvent
        {
            get { return Event != null; }
        }

        public IUtcDateConverter DateConverter { get; set; }

        public HtmlContentProperties ContentProperties_Body_Html { get; set; }

        public FileContentProperties ContentProperties_Body_ImageFile { get; set; }

        public FileContentProperties ContentProperties_Body_DocumentFile { get; set; }

        public ImageContentProperties ContentProperties_Image { get; set; }

        public FileContentProperties ContentProperties_ImageFile { get; set; }
    }
}
