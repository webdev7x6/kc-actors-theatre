using System.Web.Mvc;
using KCActorsTheatre.Web.Mvc;

namespace KCActorsTheatre.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new KCActorsTheatreHandleErrorAttribute());
        }
    }
}
