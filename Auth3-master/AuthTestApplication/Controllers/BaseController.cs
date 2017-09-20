using System.Web.Mvc;
using AuthTestApplication.Models;

namespace AuthTestApplication.Controllers
{
    public class BaseController : Controller
    {
        #region Helpers
        public ActionResult RedirectToStartPage(ApplicationUser user, string returnUrl)
        {
            switch (user.Role)
            {
                case ApplicationRole.User:
                    return RedirectToAction("UserWelcome", "Home");
                case ApplicationRole.Admin:
                    return RedirectToAction("AdminWelcome", "Home");
                default:
                    return RedirectToAction("Login", "Account");
            }
        }
        #endregion
    }
}