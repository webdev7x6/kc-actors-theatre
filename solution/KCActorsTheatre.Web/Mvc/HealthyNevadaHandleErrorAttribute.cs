using System;
using System.Web.Mvc;
using Clickfarm.AppFramework.Logging;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Web.ViewModels;

namespace KCActorsTheatre.Web.Mvc
{
    public class KCActorsTheatreHandleErrorAttribute : CmsHandleErrorAttribute
    {
        public KCActorsTheatreHandleErrorAttribute() : base() { }

        protected override void LogError(string message, Exception exception)
        {
            DependencyResolver.Current.GetService<ILogger>().Error(message, exception);
        }

        protected override ActionResult ActionResult(ExceptionContext filterContext, HandleErrorInfo model)
        {
            return new JsonResult
            {
                Data = new ErrorViewModel(null, model.Exception),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
