using Clickfarm.Cms.Core;
using KCActorsTheatre.Contract;


namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.PersonModel
{
    public class EditPersonViewModel
    {
        public Person Person { get; set; }

        public bool HasPerson
        {
            get { return Person != null; }
        }

        public IUtcDateConverter DateConverter { get; set; }

        public HtmlContentProperties ContentProperties_Body_Html { get; set; }

        public FileContentProperties ContentProperties_Body_ImageFile { get; set; }

        public FileContentProperties ContentProperties_Body_DocumentFile { get; set; }

        public ImageContentProperties ContentProperties_Image { get; set; }

        public FileContentProperties ContentProperties_ImageFile { get; set; }
    }
}
