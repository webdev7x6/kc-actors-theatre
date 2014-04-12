using Clickfarm.Cms.Core;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
using System.Web;
using KCActorsTheatre.Blogs;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.PostCategories
{
    public class EditViewModel
    {
        public PostCategory PostCategory { get; set; }

        public bool HasPostCategory
        {
            get { return PostCategory != null; }
        }

        public IUtcDateConverter DateConverter { get; set; }
    }
}