using System.Web.Mvc;
using System.Web.Routing;
using Clickfarm.Cms.Mvc;

namespace KCActorsTheatre.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.Add(DependencyResolver.Current.GetService<CmsRoute>());

            routes.MapRoute(
                "NewComment",
                "NewComment",
                new { controller = "Blog", action = "NewComment" }
            );

            routes.MapRoute(
                "NewSignUp",
                "NewSignUp",
                new { controller = "Home", action = "NewSignUp" },
                new[] { "KCActorsTheatre.Web.Controllers" }
            );

            routes.MapRoute("ControllerActions",
                "{controller}/{action}",
                new { controller = "Home", action = "Index" }, new[] { "KCActorsTheatre.Web.Controllers" });

        }
    }
}