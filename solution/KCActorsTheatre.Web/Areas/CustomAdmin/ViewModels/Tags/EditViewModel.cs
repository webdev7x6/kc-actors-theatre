using Clickfarm.AppFramework.Extensions;
using Clickfarm.Cms.Core;
using KCActorsTheatre.Announcements;
using KCActorsTheatre.Tags;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Tags
{
    public class EditViewModel
    {
        public Tag Tag { get; set; }

        public bool HasTag
        {
            get { return Tag != null; }
        }

        public IUtcDateConverter DateConverter { get; set; }

        public string GetAssociatedAnnouncments()
        {
            var items = Tag.Announcements.Where(p => p.AnnouncementType == AnnouncementType.Announcement);
            return MakeAnnouncementList(items);
        }

        public string GetAssociatedOfficialAnnouncments()
        {
            var items = Tag.Announcements.Where(p => p.AnnouncementType == AnnouncementType.Official);
            return MakeAnnouncementList(items);
        }

        public string GetAssociatedMissionStories()
        {
            var items = Tag.MissionStories;

            var sb = new StringBuilder();
            if (items.Any())
            {
                foreach (var article in items.OrderBy(p => p.Title))
                    sb.AppendFormat("<li>{0}</li>", article.Title);

                sb.Insert(0, "<ul>").Append("</ul>");
            }
            else
            {
                sb.AppendLine("<div>None</div>");
            }

            return sb.ToString();
        }

        public string GetAssociatedResources()
        {
            var sb = new StringBuilder();
            var items = Tag.Resources;
            if (items.Any())
            {
                foreach (var item in items.OrderBy(p => p.Title))
                    sb.AppendFormat("<li>{0}: {1}</li>", EnumExtensions.GetDescription(item.ResourceType), item.Title);

                sb.Insert(0, "<ul>").Append("</ul>");
            }
            else
            {
                sb.AppendLine("<div>None</div>");
            }

            return sb.ToString();
        }

        private string MakeAnnouncementList(IEnumerable<Announcement> items)
        {
            var sb = new StringBuilder();
            if (items.Any())
            {
                foreach (var article in items.OrderBy(p => p.Title))
                    sb.AppendFormat("<li>{0}</li>", article.Title);

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