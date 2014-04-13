using Clickfarm.Cms.Core;
using KCActorsTheatre.Blog;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Blog
{
    public class EditPostViewModel
    {
        public Post Post { get; set; }

        public bool HasPost
        {
            get { return Post != null; }
        }

        public IUtcDateConverter DateConverter { get; set; }

        public HtmlContentProperties ContentProperties_Body_Html { get; set; }

        public ImageContentProperties ContentProperties_Image { get; set; }

        public FileContentProperties ContentProperties_ImageFile { get; set; }
    }
}
