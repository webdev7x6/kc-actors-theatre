using Clickfarm.Cms.Core;
using System.ComponentModel.DataAnnotations;

namespace KCActorsTheatre.Cms.ContentTypes
{
    public class AlertContent : HyperlinkContent
    {
        public string AlertDescription { get; set; }
        public string EnglishLinkText { get; set; }
        public string EnglishLinkURL { get; set; }
        public string SpanishLinkText { get; set; }
        public string SpanishLinkURL { get; set; }
        public string FrenchLinkText { get; set; }
        public string FrenchLinkURL { get; set; }

        public override string ToHtmlString()
        {
            return base.ToHtmlString();
        }
    }
}