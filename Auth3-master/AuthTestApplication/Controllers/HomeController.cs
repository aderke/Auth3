using System.Web.Mvc;
using AuthTestApplication.Managers;

namespace AuthTestApplication.Controllers
{
    public class HomeController : BaseController
    {
        private CustomerUserManager CustomUserManager { get; set; }

        public HomeController(): this(new CustomerUserManager())
        {
        }

        public HomeController(CustomerUserManager customUserManager)
        {
            CustomUserManager = customUserManager;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return RedirectToAction("Login", "Account");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AdminWelcome()
        {
            var users = RegisterUserSection.GetConfig().GetUsers();

            ViewBag.Message = "Admin welcome page";

            return View(users);
        }

        [Authorize(Roles = "User")]
        public ActionResult UserWelcome()
        {
            ViewBag.Message = "User welcome page";

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public JsonResult GenerateKey(string text)
        {
            var model = CustomUserManager.EncryptDecrypt(text);

            return Json(new { decrypted = model.Decrypted, encrypted = model.Encrypted });
        }
    }
}