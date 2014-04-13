using Clickfarm.Cms.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCActorsTheatre.Cms.ContentTypes
{
    public class RotatorImageContent : HyperlinkContent
    {
        /// <summary>
        /// Gets or sets the title of the slide.
        /// </summary>
        /// <value>
        /// The slide title.
        /// </value>
        [StringLength(250)]
        [Display(Name = "Title")]
        public string SlideTitle { get; set; }

        /// <summary>
        /// Gets or sets the caption of the slide.
        /// </summary>
        /// <value>
        /// The slide caption.
        /// </value>
        [StringLength(1000)]
        [Display(Name = "Caption")]
        public string SlideCaption { get; set; }

        public override string ToHtmlString()
        {
            return base.ToHtmlString();
        }
    }

    public class RotatorVideoContent : Content
    {
        [Display(Name = "YouTube Video Key")]
        [StringLength(50)]
        public string YouTubeKey { get; set; }

        [Display(Name = "Title")]
        [StringLength(250)]
        public string Title { get; set; }

        [Display(Name = "Caption")]
        [StringLength(1000)]
        public string Caption { get; set; }

        public override string ToHtmlString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("youtube html goes here");
            return sb.ToString();
        }
    }
}