using System.Text;
using System.Web;
using Clickfarm.Cms.Core;

namespace KCActorsTheatre.Cms.ContentTypes
{
    public class HeroImageContent : ImageContent
    {
        public override string ToHtmlString()
        {
			var sb = new StringBuilder();
			sb.AppendFormat("<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" />", HttpUtility.HtmlAttributeEncode(ImageUrl), HttpUtility.HtmlAttributeEncode(Title));
			return sb.ToString();
        }
    }
}