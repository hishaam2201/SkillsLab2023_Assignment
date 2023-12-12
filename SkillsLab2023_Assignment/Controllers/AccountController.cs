using BusinessLayer.Services.AccountService;
using DAL.Models;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Account account)
        {
            bool isValid = _accountService.AuthenticateLoginCredentials(account.Email, account.Password);

            if (isValid)
            {
                Session["isAuthenticated"] = true;
                Session["CurrentUser"] = account.Email;
                Session["CurrentRole"] = "Employee"; // Currently logging in as Employee by default
                return Json(new { success = true, message = "Login Successful", redirectUrl = "/Home/Index" });
            }
            else
            {
                return Json(new { success = false, message = "Invalid email or password" });
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user)
        {
            bool isRegistered = _accountService.Register(user);
            if (isRegistered)
            {
                Session["isAuthenticated"] = true;
                Session["CurrentUser"] = user;
                Session["CurrentRole"] = "Employee";
                return Json(new { success = true, message = "Registration Successful", redirectUrl = "/Home/Index" });
            }
            else
            {
                return Json(new { success = false, message = "Email already exists!" });
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}