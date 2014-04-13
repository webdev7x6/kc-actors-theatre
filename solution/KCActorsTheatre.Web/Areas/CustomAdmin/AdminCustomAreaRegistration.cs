using System.Web.Mvc;

namespace KCActorsTheatre.Web.Areas.CustomAdmin
{
    public class AdminCustomAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "CustomAdmin"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "CustomAdmin_default",
                "CustomAdmin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "KCActorsTheatre.Web.Areas.CustomAdmin.Controllers" }
            );
        }
    }
}