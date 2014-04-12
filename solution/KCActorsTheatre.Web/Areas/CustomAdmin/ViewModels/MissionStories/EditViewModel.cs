using Clickfarm.Cms.Core;
using KCActorsTheatre.MissionStories;
using System;
using System.Linq;
using System.Text;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.MissionStories
{
    public class EditViewModel
    {
        public MissionStory MissionStory { get; set; }

        public bool HasMissionStory
        {
            get { return MissionStory != null; }
        }

        public IUtcDateConverter DateConverter { get; set; }

        public ImageContentProperties ContentProperties_ThumbnailImage { get; set; }

        public FileContentProperties ContentProperties_ThumbnailImageFile { get; set; }

        public ImageContentProperties ContentProperties_Image { get; set; }

        public FileContentProperties ContentProperties_ImageFile { get; set; }

        public string GetTagsAsList(bool forHtml = true)
        {
            var sb = new StringBuilder();
            var tagList = MissionStory.Tags.ToList();
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