using System.Web;
using System.Web.Mvc;
using AuthTestApplication.Filters;

namespace AuthTestApplication
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new DosPreventAttribute());
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CheckLogoutAttribute());
        }
    }
}
