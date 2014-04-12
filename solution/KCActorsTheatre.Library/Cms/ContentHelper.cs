using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Clickfarm.Cms.Core;

namespace KCActorsTheatre.Cms
{
    public static class ContentHelper
    {
        private static Random rand = new Random();

        public static IHtmlString TechMenu(Menu menu)
        {
            StringBuilder sb = new StringBuilder();
            var menuItems = menu.TopLevelItems.OrderBy(mi => mi.DisplayOrder);
            MenuAsList(sb, menuItems, "nav-tech-links");
            sb.Append("<ul id=\"nav-tech-inputs\">");
            foreach (var mi in menuItems)
            {
                sb.AppendFormat(
                    "<li><input type=\"radio\" id=\"tech-{0}\" name=\"nav-tech-chooser\" data-cms-key=\"{1}\" /><label for=\"tech-{0}\">{2}</label></li>",
                    rand.Next(),
                    mi.HasUrl && mi.Url.HasPage ? mi.Url.Page.CmsKey : string.Empty,
                    mi.Title
                );
            }
            sb.Append("</ul>");
            return new HtmlString(sb.ToString());
        }

        public static IHtmlString EnvMenu(Menu menu)
        {
            StringBuilder sb = new StringBuilder();
            MenuAsList(sb, menu.TopLevelItems.OrderBy(mi => mi.DisplayOrder), "nav-env-links", "horizontal-menu");
            return new HtmlString(sb.ToString());
        }

        public static IHtmlString TechEnvMenu(Menu menu)
        {
            StringBuilder sb = new StringBuilder();
            MenuAsList(sb, menu.TopLevelItems.OrderBy(mi => mi.DisplayOrder).Select(mi => mi.Children.First()), "nav-tech-env-links");
            return new HtmlString(sb.ToString());
        }

        private static void MenuAsList(StringBuilder sb, IEnumerable<MenuItem> menuItems, string listID, string listCssClass = null)
        {
            sb.Append("<ul");
            if (!string.IsNullOrWhiteSpace(listID))
            {
                sb.AppendFormat(" id=\"{0}\"", listID);
            }
            if (!string.IsNullOrWhiteSpace(listCssClass))
            {
                sb.AppendFormat(" class=\"{0}\"", listCssClass);
            }
            sb.Append(">");
            foreach (var mi in menuItems)
            {
                sb.Append("<li>");
                RenderLink(sb, mi);
                sb.Append("</li>");
            }
            sb.Append("</ul>");
        }

        private static void RenderLink(StringBuilder sb, MenuItem item, string cssClass = null)
        {
            if ((item.HasUrl && !string.IsNullOrWhiteSpace(item.Url.Path)) || item.HasExternalLinkUrl)
            {
                sb.Append("<a href=\"");
                if (item.HasExternalLinkUrl)
                {
                    sb.AppendFormat("{0}\" target=\"_blank", item.ExternalLinkUrl);
                }
                else
                {
                    sb.Append("javascript:void(0);");
                    if (item.HasUrl)
                    {
                        sb.AppendFormat("\" data-url=\"{0}", item.Url.Path);
                        if (item.Url.HasPage && !string.IsNullOrWhiteSpace(item.Url.Page.CmsKey))
                        {
                            sb.AppendFormat("\" data-cms-key=\"{0}", item.Url.Page.CmsKey);
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(cssClass))
                {
                    sb.AppendFormat("\" class=\"{0}", cssClass);
                }
                sb.AppendFormat("\">{0}</a>", item.Title);
            }
            else
            {
                sb.Append(item.Title);
            }
        }

        public static string FormatPlainTextAsHtml(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            return
                "<p>" +
                text
                    .Replace("\r\n\r\n", "</p><p>")
                    .Replace("\r\r", "</p><p>")
                    .Replace("\n\n", "</p><p>")
                    .Replace("\r\n", "<br/>")
                    .Replace("\r", "<br/>")
                    .Replace("\n", "<br/>")
                    .Replace("<p><br/>", "<p>")
                    .Replace("<br/></p>", "</p>")
                    .Replace("<p></p>", string.Empty)
                    +
                "</p>"
            ;
        }
    }
}
