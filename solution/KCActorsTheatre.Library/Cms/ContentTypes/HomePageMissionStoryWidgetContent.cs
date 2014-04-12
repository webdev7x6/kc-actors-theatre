using Clickfarm.Cms.Core;
using System.ComponentModel.DataAnnotations;

namespace KCActorsTheatre.Cms.ContentTypes
{
    public class HomePageMissionStoryWidgetContent : Content
    {
        [StringLength(50)]
        [Display(Name = "Title")]
        public string MissionStoryWidgetTitle { get; set; }

        [StringLength(200)]
        [Display(Name = "Text")]
        public string MissionStoryWidgetText { get; set; }

        [StringLength(50)]
        [Display(Name = "Text")]
        public string MissionStoryWidgetLinkText { get; set; }

        public override string ToHtmlString()
        {
            return string.Empty;
        }
    }
}