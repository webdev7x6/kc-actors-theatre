using Clickfarm.Cms.Core;
using KCActorsTheatre.Locations;
using System.Linq;
using System.Text;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.MissionFields
{
    public class EditViewModel
    {
        public MissionField MissionField { get; set; }

        public bool HasMissionField
        {
            get { return MissionField != null; }
        }

        public IUtcDateConverter DateConverter { get; set; }

        public string GetAssociatedMissionCenters()
        {
            var items = MissionField.MissionCenters;

            var sb = new StringBuilder();
            if (items.Any())
            {
                foreach (var item in items.OrderBy(p => p.Name))
                    sb.AppendFormat("<li>{0}</li>", item.Name);

                sb.Insert(0, "<ul>").Append("</ul>");
            }
            else
            {
                sb.AppendLine("<div>None</div>");
            }

            return sb.ToString();
        }
    }
}