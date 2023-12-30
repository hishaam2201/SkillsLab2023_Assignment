
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
        public HomeController(ITrainingService trainingService)
        {
            _trainingService = trainingService;
        }

        [CustomAuthorization(RoleEnum.Employee)]
        public ActionResult EmployeeDashboard()
        {
            return View();
        }

        [CustomAuthorization(RoleEnum.Manager)]
        public ActionResult ManagerDashboard()
        {
            return View();
        }

        [CustomAuthorization(RoleEnum.Administrator)]
        public async Task<ActionResult> AdministratorDashboard()
        {
            // TODO: Background job to check for deadline of application, if expired then do not display
            List<TrainingDTO> trainingList = (await _trainingService.GetAllTrainingsAsync()).ToList();
            return View(trainingList);
        }
    }
}