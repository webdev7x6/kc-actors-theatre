using Clickfarm.Cms.Core;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
using System.Web;
using KCActorsTheatre.Announcements;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Announcements
{
    public class EditViewModel
    {
        public Announcement Announcement { get; set; }

        public bool HasAnnouncement
        {
            get { return Announcement != null; }
        }

        public IUtcDateConverter DateConverter { get; set; }

        public string GetTagsAsList(bool forHtml = true)
        {
            var sb = new StringBuilder();
            var tagList = Announcement.Tags.ToList();
            if (tagList.Any())
            {
                string line = forHtml ? "<li data-tag-id=\"{1}\">{0}</li>" : ("{0}" + Environment.NewLine);
                foreach (var m in tagList.OrderBy(p => p.Name))
                {
                    sb.AppendFormat(line, m.Name, m.TagID);
                }
                if (forHtml)
                {
                    sb.Insert(0, "<ul>")
                        .Append("</ul>");
                }
            }

            return sb.ToString();
        }
    }
}