﻿using BusinessLayer.Services.AccountService;
using DAL.DTO;
using DAL.Models;
using Framework.Enums;
using Framework.StaticClass;
using SkillsLab2023_Assignment.Custom;
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
            bool isValid = await _accountService.AuthenticateLoginCredentialsAsync(loginViewModel.Email, loginViewModel.Password);
            if (isValid)
            {
                SessionManager.Email = loginViewModel.Email;
                return Json(new { success = true, message = "Login Successful", redirectUrl = Url.Action("ChooseRole", "Account") });
            }
            else
            {
                return Json(new { success = false, message = "Invalid email or password" });
            }
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

            if (user != null)
            {
                SessionManager.Remove(email);
                SessionManager.CurrentUser = user;
                SessionManager.UserRole = ((RoleEnum)selectedRole).ToString();
                string dashboardAction = Extensions.GetDashboardAction(selectedRole);

                return Json(new
                {
                    success = true,
                    message = "Redirecting to dashboard...",
                    redirectUrl = Url.Action(dashboardAction, "Home")
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "User data not found",
                    redirectUrl = Url.Action("Login", "Account")
                });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetAllManagersFromDepartment(int departmentId)
        {
            List<ManagerDTO> managerDTO = (await _accountService.GetAllManagersFromDepartmentAsync(departmentId)).ToList();
            return Json(new { success = true, managers = managerDTO }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// // When user logs in by default he is an employee, when admin changes role then he will have more roles available
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel registerViewModel)
        {
            User user = new User
            {
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                MobileNumber = registerViewModel.MobileNumber,
                NationalIdentityCard = registerViewModel.NationalIdentityCard,
                DepartmentId = registerViewModel.DepartmentId,
                ManagerId = registerViewModel.ManagerId,
                Email = registerViewModel.Email
            };
            bool isRegistered = await _accountService.RegisterUserAsync(user, registerViewModel.Email, registerViewModel.Password);

            if (isRegistered)
            {
                UserDTO userData = await _accountService.GetUserDataAsync(user.Email, (byte)RoleEnum.Employee);
                SessionManager.CurrentUser = userData;
                SessionManager.UserRole = RoleEnum.Employee.ToString();

                string dashboardAction = Extensions.GetDashboardAction((byte)RoleEnum.Employee);
                return Json(new { success = true, message = "Registration Successful", 
                    redirectUrl = Url.Action(dashboardAction, "Home") });
            }
            else
            {
                return Json(new { success = false, message = "Email already exists!" });
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

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult LogOut()
        {
            SessionManager.ClearSession();
            return RedirectToAction("Login", "Account");
        }
    }
}