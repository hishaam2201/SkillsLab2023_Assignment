using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Services.AccountService;
using System;
using System.Security.Principal;
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
                Session["CurrentUser"] = account.Email; // Testing purposes
                Session["CurrentRole"] = "Employee"; // Testing purposes
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
            // TODO: When registering, store user data in session
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

        [HttpGet]
        public ActionResult LogOut()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }
    }
}