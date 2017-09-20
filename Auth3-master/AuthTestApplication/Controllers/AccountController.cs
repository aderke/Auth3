using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AuthTestApplication.Managers;
using AuthTestApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace AuthTestApplication.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private const int DelayInterval = 5 * 60 * 1000;
        private CustomerUserManager CustomUserManager { get; set; }

        public AccountController(): this(new CustomerUserManager())
        {
        }

        public AccountController(CustomerUserManager customUserManager)
        {
            CustomUserManager = customUserManager;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            int delay;
            if (HttpContext.Application[Request.UserHostAddress] != null)
            {
                delay = (int) HttpContext.Application[Request.UserHostAddress];

                if(delay > 5)
                    await Task.Delay(DelayInterval);
            }

            if (ModelState.IsValid)
            {
                var user = await CustomUserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user);

                    if (HttpContext.Application[Request.UserHostAddress] != null)
                    {
                        HttpContext.Application.Remove(Request.UserHostAddress);
                    }

                    return RedirectToStartPage(user, returnUrl);
                }
                else
                {
                    if (HttpContext.Application[Request.UserHostAddress] == null)
                    {
                        delay = 1;
                    }
                    else
                    {
                        delay = ((int) HttpContext.Application[Request.UserHostAddress]) + 1;
                    }

                    HttpContext.Application[Request.UserHostAddress] = delay;

                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }
            
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult UserLogoff(string username)
        {
            var keys = HttpContext.Application.AllKeys;
            var userKey = keys.FirstOrDefault(k => k == username);

            if (userKey == null)
            {
                HttpContext.Application.Add(username, true);
            }

            return Json(new { success = 1 });
        }

        // POST: /Account/LogOff
        [HttpPost]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public ActionResult SignOut()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && CustomUserManager != null)
            {
                CustomUserManager.Dispose();
                CustomUserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            var identity = await CustomUserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, user.Role.ToString()));

            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        #endregion
    }
}