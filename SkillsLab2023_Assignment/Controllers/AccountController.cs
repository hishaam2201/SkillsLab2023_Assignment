using BusinessLayer.Services.AccountService;
using DAL.DTO;
using DAL.Models;
using Framework.Enums;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult Login(LoginDTO loginDTO)
        {
            Account account = new Account
            {
                Email = loginDTO.Email,
                Password = loginDTO.Password,
            };
            bool isValid = _accountService.AuthenticateLoginCredentials(account.Email, account.Password);

            if (isValid)
            {
                Session["isAuthenticated"] = true;
                Session["CurrentUser"] = account.Email;
                Session["UserRole"] = "Employee"; // Currently logging in as Employee by default
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

        [HttpGet]
        public JsonResult GetAllDepartments()
        {
            List<DepartmentDTO> departmentDTO = _accountService.GetAllDepartments().ToList();
            return Json(new { success = true, departments = departmentDTO }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Register(RegistrationDTO registrationDTO)
        {
            User user = new User
            {
                FirstName = registrationDTO.FirstName,
                LastName = registrationDTO.LastName,
                MobileNumber = registrationDTO.MobileNumber,
                NationalIdentityCard = registrationDTO.NationalIdentityCard,
                DepartmentId = registrationDTO.DepartmentId,
                ManagerId = registrationDTO.ManagerId
            };
            bool isRegistered = _accountService.Register(user, registrationDTO.Email, registrationDTO.Password);

            if (isRegistered)
            {
                Session["isAuthenticated"] = true;
                Session["CurrentUser"] = user;
                Session["UserRole"] = (int)RolesEnum.Employee;
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