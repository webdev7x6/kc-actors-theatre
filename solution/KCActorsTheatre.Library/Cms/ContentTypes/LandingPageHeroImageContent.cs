using Clickfarm.Cms.Core;
using System.ComponentModel.DataAnnotations;

namespace KCActorsTheatre.Cms.ContentTypes
{
    public class LandingPageHeroImageContent : HyperlinkContent
    {
        public string HeroTitle { get; set; }
        public string HeroCaption { get; set; }

        public override string ToHtmlString()
        {
            return base.ToHtmlString();
        }
    }
}