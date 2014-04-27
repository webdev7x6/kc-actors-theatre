using Clickfarm.Cms.Core;
using KCActorsTheatre.Contract;
using System;
using System.Linq;
using System.Text;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Season
{
    public class EditSeasonViewModel
    {
        public SeasonInfo Season { get; set; }

        public bool HasSeason
        {
            get { return Season != null; }
        }

        public IUtcDateConverter DateConverter { get; set; }

        public HtmlContentProperties ContentProperties_Body_Html { get; set; }

        public FileContentProperties ContentProperties_Body_ImageFile { get; set; }

        public FileContentProperties ContentProperties_Body_DocumentFile { get; set; }

        public ImageContentProperties ContentProperties_Image { get; set; }

        public FileContentProperties ContentProperties_ImageFile { get; set; }

        public string GetShowsAsList(bool forHtml = true)
        {
            var sb = new StringBuilder();
            var tagList = Season.Shows.ToList();
            if (tagList.Any())
            {
                string line = forHtml ? "<li data-show-id=\"{1}\">{0}</li>" : ("{0}" + Environment.NewLine);
                foreach (var m in tagList.OrderBy(p => p.Title))
                {
                    sb.AppendFormat(line, m.Title, m.SeasonID);
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
