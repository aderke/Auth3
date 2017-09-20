using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AuthTestApplication.Filters
{
    public class CheckLogoutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var userName = HttpContext.Current.User.Identity.Name;
            var keys = HttpContext.Current.Application.AllKeys;

            var userKey = keys.FirstOrDefault(k => k == userName);
            
            if (userKey != null && (bool) HttpContext.Current.Application[userName])
            {
                base.OnActionExecuting(filterContext);
                HttpContext.Current.Application.Remove(userName);
                HttpContext.Current.Session.Abandon();

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Account",
                    action = "SignOut"
                })); 
            }
        }
    }
}
