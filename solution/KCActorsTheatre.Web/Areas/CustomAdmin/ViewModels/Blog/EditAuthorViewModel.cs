using Clickfarm.Cms.Core;
using KCActorsTheatre.Blog;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Blog
{
    public class EditAuthorViewModel
    {
        public Author Author { get; set; }

        public bool HasAuthor
        {
            get { return Author != null; }
        }

        public IUtcDateConverter DateConverter { get; set; }

        public ImageContentProperties ContentProperties_Image { get; set; }

        public FileContentProperties ContentProperties_ImageFile { get; set; }
    }
}
