using BusinessLayer.Services.ApplicationService;
using DAL.DTO;
using SkillsLab2023_Assignment.Custom;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Controllers
{
    [UserSession]
    public class ApplicationController : Controller
    {
        private readonly IApplicationService _applicationService;
        public ApplicationController(IApplicationService applicationService) 
        {
            _applicationService = applicationService;
        }

        [HttpPost]
        public async Task<ActionResult> Enroll(HttpPostedFileBase file)
        {
            UserDTO user = Session["CurrentUser"] as UserDTO;
            short userId = user.Id;
            /*if (file != null && file.ContentLength > 0)
            {
                Stream stream = file.InputStream;
                string downloadLink = await _applicationService.Upload(stream, file.FileName);
            }*/
            return RedirectToAction("EmployeeDashboard", "Home");
        }
    }
}