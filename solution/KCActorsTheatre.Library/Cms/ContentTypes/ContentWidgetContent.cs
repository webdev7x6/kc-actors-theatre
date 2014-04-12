using Clickfarm.Cms.Core;
using System.ComponentModel.DataAnnotations;

namespace KCActorsTheatre.Cms.ContentTypes
{
    public class ContentWidgetContent : HyperlinkContent
    {
        [StringLength(50)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [StringLength(150)]
        [Display(Name = "Text")]
        public string Text { get; set; }

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        public override string ToHtmlString()
        {
            return base.ToHtmlString();
        }
    }
}