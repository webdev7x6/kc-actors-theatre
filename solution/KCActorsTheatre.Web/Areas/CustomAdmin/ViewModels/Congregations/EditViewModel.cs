using Clickfarm.Cms.Core;
using KCActorsTheatre.Locations;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
using System.Web;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Congregations
{
    public class EditViewModel
    {
        public Congregation Congregation { get; set; }

        public bool HasCongregation
        {
            get { return Congregation != null; }
        }

        public IUtcDateConverter DateConverter { get; set; }

        public ImageContentProperties ContentProperties_ThumbnailImage { get; set; }

        public FileContentProperties ContentProperties_ThumbnailImageFile { get; set; }

        public FileContentProperties ContentProperties_ResourceFile { get; set; }

        public string GetCongregationContactsAsList(bool forHtml = true)
        {
            var sb = new StringBuilder();
            var items = Congregation.CongregationContacts.OrderBy(p => p.DisplayOrder).ToList();
            if (items.Any())
            {
                string line = forHtml ? "<li data-contact-id=\"{1}\" data-contact-position=\"{2}\"  data-contact-email=\"{3}\" data-contact-phone=\"{4}\" >{0}</li>" : ("{0}" + Environment.NewLine);
                foreach (var item in items)
                {
                    sb.AppendFormat(line, item.Name, item.CongregationContactID, item.Position, item.Email, item.Phone);
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