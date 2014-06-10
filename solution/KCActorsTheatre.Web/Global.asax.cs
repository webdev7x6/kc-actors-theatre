using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Clickfarm.AppFramework.Logging;

namespace KCActorsTheatre.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();

            CmsConfig.Bootstrap();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error()
        {
            DependencyResolver.Current.GetService<ILogger>().LogWeb(
                DependencyResolver.Current.GetService<HttpContextBase>(),
                LogLevel.Error,
                "An unhandled exception occurred on KCActorsTheatre.com.",
                Server.GetLastError().GetBaseException()
            );
        }
    }
}
