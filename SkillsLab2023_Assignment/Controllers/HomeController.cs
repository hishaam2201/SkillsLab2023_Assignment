using BusinessLayer.Services.ApplicationService;
using BusinessLayer.Services.TrainingService;
using DAL.DTO;
using Framework.Enums;
using SkillsLab2023_Assignment.Custom;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Controllers
{
    [UserSession]
    public class HomeController : Controller
    {
        private readonly ITrainingService _trainingService;
        private readonly IApplicationService _applicationService;
        public HomeController(ITrainingService trainingService, IApplicationService applicationService)
        {
            _trainingService = trainingService;
            _applicationService = applicationService;
        }

        [HttpGet, CustomAuthorization(RoleEnum.Employee)]
        public async Task<ActionResult> EmployeeDashboard()
        {
            short userId = SessionManager.CurrentUser.Id;
            var userApplications = (await _applicationService.GetApplicationByUserId(userId))?.ToList() ?? new List<UserApplicationDTO>();
            return View(userApplications);
        }

        [HttpGet, CustomAuthorization(RoleEnum.Manager)]
        public async Task<ActionResult> ManagerDashboard()
        {
            short managerId = SessionManager.CurrentUser.Id;
            List<ApplicationDTO> applications = (await _applicationService.GetApplicationsAsync(managerId)).ToList();
            return View(applications);
        }

        [HttpGet, CustomAuthorization(RoleEnum.Administrator)]
        public async Task<ActionResult> AdministratorDashboard()
        {
            var trainingList = (await _trainingService.GetAllTrainingsAsync())?.ToList() ?? new List<TrainingDTO>();
            return View(trainingList);
        }
    }
}