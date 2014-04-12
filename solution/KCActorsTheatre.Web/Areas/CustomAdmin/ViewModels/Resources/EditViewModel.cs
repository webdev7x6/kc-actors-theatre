using Clickfarm.Cms.Core;
using KCActorsTheatre.Resources;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
using System.Web;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Resources
{
    public class EditViewModel
    {
        public EditViewModel()
        {
            // Resource.ThumbnailImageURL width and height
            ContentProperties_ThumbnailImage = new ImageContentProperties
            {
                ExactWidth = 200,
                ExactHeight = 200
            };

            // Resource.ThumbnailImageURL properties
            ContentProperties_ThumbnailImageFile = new FileContentProperties
            {
                RootFolder = "/common/cms/resource/thumbnail",
                DefaultSubfolder = "",
                MediaTypes = new string[] { "image/" }
            };

            // Resource.ResourceType.Audio properties
            ContentProperties_AudioFile = new FileContentProperties
            {
                RootFolder = "/common/cms/resource/audio",
                DefaultSubfolder = "",
                FileExtensions = new[] 
                { 
                    "mp3",
                },
                    MediaTypes = new[] 
                { 
                    "audio/mpeg", 
                }
            };

            // Resource.ResourceType.Document properties
            ContentProperties_DocumentFile = new FileContentProperties
            {
                RootFolder = "/common/cms/resource/document",
                DefaultSubfolder = "",
                FileExtensions = new[] 
                { 
                    "doc", 
                    "docx",
                    "pdf", 
                    "xls", 
                    "xlsx", 
                    "zip" 
                },
                MediaTypes = new[] 
                { 
                    "application/pdf", 
                    "application/vnd.ms-excel", 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    "application/msword", 
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document", 
                    "application/zip", 
                    "application/x-zip-compressed" 
                }
            };
        }

        public Resource Resource { get; set; }
        public bool HasResource { get { return Resource != null; } }

        public IUtcDateConverter DateConverter { get; set; }
        public ImageContentProperties ContentProperties_ThumbnailImage { get; set; }
        public FileContentProperties ContentProperties_ThumbnailImageFile { get; set; }
        public FileContentProperties ContentProperties_AudioFile { get; set; }
        public FileContentProperties ContentProperties_DocumentFile { get; set; }

        public string GetTagsAsList(bool forHtml = true)
        {
            var sb = new StringBuilder();
            var tagList = Resource.Tags.ToList();
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