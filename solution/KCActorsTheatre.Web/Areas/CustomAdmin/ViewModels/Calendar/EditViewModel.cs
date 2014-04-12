using Clickfarm.Cms.Core;
using KCActorsTheatre.Calendar;
using System;
using System.Linq;
using System.Text;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Calendar
{
    public class EditViewModel
    {
        public CalendarEvent CalendarEvent { get; set; }

        public bool HasCalendarEvent
        {
            get { return CalendarEvent != null; }
        }

        public IUtcDateConverter DateConverter { get; set; }

        public ImageContentProperties ContentProperties_ThumbnailImage { get; set; }

        public FileContentProperties ContentProperties_ThumbnailImageFile { get; set; }

        public ImageContentProperties ContentProperties_Image { get; set; }

        public FileContentProperties ContentProperties_ImageFile { get; set; }

        public string GetTagsAsList(bool forHtml = true)
        {
            var sb = new StringBuilder();
            var tagList = CalendarEvent.Tags.ToList();
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