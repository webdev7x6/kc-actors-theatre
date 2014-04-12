using Clickfarm.Cms.Core;
using System.ComponentModel.DataAnnotations;

namespace KCActorsTheatre.Cms.ContentTypes
{
    public class RotatorImageContent : HyperlinkContent
    {
        [StringLength(50)]
        [Display(Name = "Title")]
        public string RotatorTitle { get; set; }

        [StringLength(250)]
        [Display(Name = "Caption")]
        public string RotatorCaption { get; set; }

        [Display(Name = "Display Order")]
        public int RotatorOrder { get; set; }

        public override string ToHtmlString()
        {
            return base.ToHtmlString();
        }
    }
}