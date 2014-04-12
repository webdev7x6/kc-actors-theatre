using System.ComponentModel.DataAnnotations;
namespace KCActorsTheatre.Cms.ContentTypes
{
    public class AnnouncementWidgetContent : TaggedWidgetContent
    {
        public override string ToHtmlString()
        {
            return string.Empty;
        }
    }
}