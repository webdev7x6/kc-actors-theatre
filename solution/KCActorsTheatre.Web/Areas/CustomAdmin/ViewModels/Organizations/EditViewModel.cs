using Clickfarm.AppFramework.Extensions;
using Clickfarm.Cms.Core;
using KCActorsTheatre.Organizations;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KCActorsTheatre.Web.Areas.CustomAdmin.ViewModels.Organizations
{
    public class EditViewModel
    {
        public Organization Organization { get; set; }

        public bool HasOrganization
        {
            get { return Organization != null; }
        }

        public IUtcDateConverter DateConverter { get; set; }
    }
}