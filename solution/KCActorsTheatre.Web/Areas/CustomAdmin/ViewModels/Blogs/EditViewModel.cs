using Clickfarm.Cms.Core;
using KCActorsTheatre.Blogs;
using System;
using System.Linq;
using System.Text;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Blogs
{
    public class EditViewModel
    {
        public Post Post { get; set; }

        public bool HasPost
        {
            get { return Post != null; }
        }

        public IUtcDateConverter DateConverter { get; set; }

        public string GetTagsAsList(bool forHtml = true)
        {
            var sb = new StringBuilder();
            var tagList = Post.Tags.ToList();
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

        public string GetCategoriesAsList(bool forHtml = true)
        {
            var sb = new StringBuilder();
            var categoryList = Post.Categories.ToList();
            if (categoryList.Any())
            {
                string line = forHtml ? "<li data-category-id=\"{1}\">{0}</li>" : ("{0}" + Environment.NewLine);
                foreach (var m in categoryList.OrderBy(p => p.Name))
                {
                    sb.AppendFormat(line, m.Name, m.PostCategoryID);
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