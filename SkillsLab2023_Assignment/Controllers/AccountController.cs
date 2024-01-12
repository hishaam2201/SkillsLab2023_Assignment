using BusinessLayer.Services.AccountService;
using DAL.DTO;
using DAL.Models;
using Framework.Enums;
using Framework.StaticClass;
using SkillsLab2023_Assignment.Custom;
using SkillsLab2023_Assignment.Mapper;
using SkillsLab2023_Assignment.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Controllers
{
    [ValidationFilter]
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
        public async Task<ActionResult> Login(LoginViewModel loginViewModel)
        {
            if (loginViewModel == null)
            {
                return Json(new { success = false, message = "Invalid input" });
            }

            OperationResult result = await _accountService.AuthenticateLoginCredentialsAsync(loginViewModel.Email, loginViewModel.Password);
            if (result.Success)
            {
                SessionManager.Email = loginViewModel.Email;
            }
            return Json(new
            {
                success = result.Success,
                message = result.Message,
                redirectUrl = result.Success ? Url.Action("ChooseRole", "Account") : null
            });
        }

        [HttpGet]
        public async Task<ActionResult> ChooseRole()
        {
            string email = SessionManager.Email;
            List<UserRoleDTO> userRoles = (await _accountService.GetUserRolesAsync(email)).ToList();
            return View(userRoles);
        }

        [HttpPost]
        public async Task<JsonResult> ChooseRole(byte selectedRole)
        {
            string email = SessionManager.Email;
            UserDTO user = await _accountService.GetUserDataAsync(email, selectedRole);
            bool isUserNotNull = user != null;
            if (isUserNotNull)
            {
                SessionManager.Remove(email);
                SessionManager.CurrentUser = user;
                SessionManager.UserRole = ((RoleEnum)selectedRole).ToString();
            }
            string dashboardAction = Extensions.GetDashboardAction(selectedRole);
            return Json(new
            {
                success = isUserNotNull,
                message = isUserNotNull ? "Redirecting to dashboard..." : "User data not found",
                redirectUrl = isUserNotNull ? Url.Action(dashboardAction, "Home") : null
            });
        }

        [HttpGet]
        public async Task<JsonResult> GetAllManagersFromDepartment(byte departmentId)
        {
            List<ManagerDTO> managerDTO = (await _accountService.GetAllManagersFromDepartmentAsync(departmentId)).ToList();
            return Json(new { success = true, managers = managerDTO }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel registerViewModel)
        {
            User user = registerViewModel.ToUser();
            OperationResult result = await _accountService.RegisterUserAsync(user, registerViewModel.Email, registerViewModel.Password);
            if (result.Success)
            {
                UserDTO userData = await _accountService.GetUserDataAsync(user.Email, (byte)RoleEnum.Employee);
                SessionManager.CurrentUser = userData;
                SessionManager.UserRole = RoleEnum.Employee.ToString();
            }
            string dashboardAction = Extensions.GetDashboardAction((byte)RoleEnum.Employee);
            return Json(new
            {
                success = result.Success,
                message = result.Message,
                redirectUrl = result.Success ? Url.Action(dashboardAction, "Home") : null
            });
        }

        [HttpGet]
        public async Task<JsonResult> GetAllDepartments()
        {
            List<DepartmentDTO> departmentDTO = (await _accountService.GetAllDepartmentsAsync()).ToList();
            return Json(new { success = true, departments = departmentDTO }, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult LogOut()
        {
            SessionManager.ClearSession();
            return RedirectToAction("Login", "Account");
        }
    }
}