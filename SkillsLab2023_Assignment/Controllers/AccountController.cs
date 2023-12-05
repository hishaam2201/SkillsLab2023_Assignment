using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Services.AccountService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
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
            try
            {
                bool isValid = _accountService.AuthenticateLoginCredentials(account.Email, account.Password);

                if (isValid)
                {
                    Session["isAuthenticated"] = true;
                    return Json(new { success = true, message = "Login Successful", redirectUrl = "/Home/Index" });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid email or password" });
                }

            }
            catch (Exception ex)
            {
                // TODO: Normally we log it here then display meaningful message to user
                return Json(new { success = false, message = "An error occurred during login: " + ex.Message });
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
            try
            {
                bool isRegistered = _accountService.Register(user);
                // TODO: When registering, store user data in session
                if (isRegistered)
                {
                    Session["isAuthenticated"] = true;
                    return Json(new { success = true, message = "Registration Successful", redirectUrl = "/Home/Index" });
                }
                else
                {
                    return Json(new { success = false, message = "Email already exists!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred during registration"});
            }
        }
    }
}