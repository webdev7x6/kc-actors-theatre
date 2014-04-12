using Clickfarm.Cms.Core;
using KCActorsTheatre.Tags;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KCActorsTheatre.Cms.ContentTypes
{
    public class TaggedWidgetContent : HyperlinkContent
    {
        [StringLength(50)]
        [Display(Name = "Widget Title")]
        public string WidgetTitle { get; set; }

        private HashSet<Tag> _tags = new HashSet<Tag>();
        public ICollection<Tag> Tags { get { return _tags; } }

        public override string ToHtmlString()
        {
            return string.Empty;
        }
    }
}