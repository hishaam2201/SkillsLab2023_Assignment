using BusinessLayer.Services.ManagerService;
using DAL.DTO;
using DAL.Repositories.ManagerRepository;
using SkillsLab2023_Assignment.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Controllers
{
    [UserSession]
    public class EnrollmentProcessController : Controller
    {
        private readonly IManagerService _managerService;
        public EnrollmentProcessController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        [HttpGet]
        [CustomAuthorization("Manager")]
        public async Task<JsonResult> ApproveApplication()
        {
            short managerId = SessionManager.CurrentUser.Id;
            List<PendingApplicationDTO> pendingApplications = (await _managerService.GetApplicationsAsync(managerId)).ToList();
            return Json(new
            {
                success = pendingApplications != null && pendingApplications.Any(),
                PendingApplications = pendingApplications
            }, JsonRequestBehavior.AllowGet);
        }
    }
}