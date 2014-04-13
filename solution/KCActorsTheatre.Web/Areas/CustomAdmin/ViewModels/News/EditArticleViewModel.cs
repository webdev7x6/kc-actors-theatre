using Clickfarm.Cms.Core;
using KCActorsTheatre.Contract;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.News
{
    public class EditArticleViewModel
    {
        public Article Article { get; set; }

        public bool HasArticle
        {
            get { return Article != null; }
        }

        public IUtcDateConverter DateConverter { get; set; }

        public HtmlContentProperties ContentProperties_Body_Html { get; set; }

        public FileContentProperties ContentProperties_Body_ImageFile { get; set; }

        public FileContentProperties ContentProperties_Body_DocumentFile { get; set; }

        public ImageContentProperties ContentProperties_Image { get; set; }

        public FileContentProperties ContentProperties_ImageFile { get; set; }
    }
}
