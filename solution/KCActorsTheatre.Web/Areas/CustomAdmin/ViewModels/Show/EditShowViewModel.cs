﻿using Clickfarm.Cms.Core;
using KCActorsTheatre.Contract;
using System;
using System.Linq;
using System.Text;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Show
{
    public class EditShowViewModel
    {
        public ShowInfo Show { get; set; }

        public bool HasShow
        {
            get { return Show != null; }
        }

        public IUtcDateConverter DateConverter { get; set; }

        public HtmlContentProperties ContentProperties_Body_Html { get; set; }

        public FileContentProperties ContentProperties_Body_ImageFile { get; set; }

        public FileContentProperties ContentProperties_Body_DocumentFile { get; set; }

        public ImageContentProperties ContentProperties_Image { get; set; }

        public FileContentProperties ContentProperties_ImageFile { get; set; }

        //public string GetPeopleAsList(bool forHtml = true)
        //{
        //    var sb = new StringBuilder();
        //    var tagList = Show.People.ToList();
        //    if (tagList.Any())
        //    {
        //        string line = forHtml ? "<li data-person-id=\"{1}\">{0}</li>" : ("{0}" + Environment.NewLine);
        //        foreach (var m in tagList.OrderBy(p => p.Name))
        //        {
        //            sb.AppendFormat(line, m.Name, m.PersonID);
        //        }
        //        if (forHtml)
        //        {
        //            sb.Insert(0, "<ul>")
        //                .Append("</ul>");
        //        }
        //    }

        //    return sb.ToString();
        //}

        public string GetImagesAsList()
        {
            var sb = new StringBuilder();
            var items = Show.Images.OrderBy(p => p.DisplayOrder).ToList();
            if (items.Any())
            {
                foreach (var item in items)
                    sb.AppendFormat("<img data-image-id=\"{0}\" src=\"{1}\" />&nbsp;&nbsp;&nbsp;&nbsp;", item.ShowImageID, item.ImageURL);
            }
            return sb.ToString();
        }

        public string GetVideosAsList()
        {
            var sb = new StringBuilder();
            var items = Show.Videos.OrderBy(p => p.DisplayOrder).ToList();
            if (items.Any())
            {
                foreach (var item in items)
                    sb.AppendFormat("<li data-video-id=\"{0}\">{1}</li>", item.ShowVideoID, item.VimeoID);

                sb.Insert(0, "<ul>")
                    .Append("</ul>");
            }
            return sb.ToString();
        }

    }
}
