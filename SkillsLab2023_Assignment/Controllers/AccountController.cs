using BusinessLayer.Services.AccountService;
using DAL.DTO;
using DAL.Models;
using Framework.Enums;
using Framework.StaticClass;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<ActionResult> Login(LoginDTO loginDTO)
        {
            bool isValid = await _accountService.AuthenticateLoginCredentialsAsync(loginDTO.Email, loginDTO.Password);

            if (isValid)
            {
                UserDTO userData = await _accountService.GetUserDataAsync(loginDTO.Email);

                if (userData != null)
                {
                    Session["isAuthenticated"] = true;
                    Session["CurrentUser"] = userData;
                    Session["UserRole"] = ((RoleEnum)userData.RoleId).ToString();

                    string dashboardAction = Extensions.GetDashboardAction(userData.RoleId);

                    return Json(new { success = true, message = "Login Successful", redirectUrl = Url.Action(dashboardAction, "Home") });
                }
                else
                {
                    return Json(new { success = false, message = "User data not found" });
                }
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
        public async Task<JsonResult> GetAllDepartments()
        {
            List<DepartmentDTO> departmentDTO = (await _accountService.GetAllDepartmentsAsync()).ToList();
            return Json(new { success = true, departments = departmentDTO }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllManagersFromDepartment(int departmentId)
        {
            List<ManagerDTO> managerDTO = (await _accountService.GetAllManagersFromDepartmentAsync(departmentId)) .ToList();
            return Json(new { success = true, managers = managerDTO }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegistrationDTO registrationDTO)
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
            bool isRegistered = await _accountService.RegisterUserAsync(user, registrationDTO.Email, registrationDTO.Password);

            if (isRegistered)
            {
                UserDTO userData = await _accountService.GetUserDataAsync(registrationDTO.Email);
                Session["isAuthenticated"] = true;
                Session["CurrentUser"] = userData;
                Session["UserRole"] = ((RoleEnum)userData.RoleId).ToString();
                return Json(new { success = true, message = "Registration Successful", redirectUrl = "/Home/EmployeeDashboard" });
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