using Clickfarm.Cms.Core;
using KCActorsTheatre.Blog;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Blog
{
    public class EditCommentViewModel
    {
        public Comment Comment { get; set; }

        public bool HasComment
        {
            get { return Comment != null; }
        }

        public IUtcDateConverter DateConverter { get; set; }

        public HtmlContentProperties ContentProperties_Body_Html { get; set; }

        public FileContentProperties ContentProperties_Body_ImageFile { get; set; }

        public FileContentProperties ContentProperties_Body_DocumentFile { get; set; }

        public ImageContentProperties ContentProperties_Image { get; set; }

        public FileContentProperties ContentProperties_ImageFile { get; set; }
    }
}
