using Clickfarm.Cms.Admin.Mvc.Areas.Admin;
using Clickfarm.Cms.Core;
using KCActorsTheatre.Data;

namespace KCActorsTheatre.Web.Areas.CustomAdmin
{
    public class KCActorsTheatreAdminControllerBase : AdminControllerBase
    {
        public KCActorsTheatreAdminControllerBase(ICmsContext context)
            : base(context)
        {
            Repository = (IKCActorsTheatreRepository)CmsContext.Repository;
        }

        protected IKCActorsTheatreRepository Repository { get; private set; }
    }
}
