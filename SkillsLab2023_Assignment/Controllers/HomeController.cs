using SkillsLab2023_Assignment_ClassLibrary.Services.UserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Controllers
{
    public class HomeController : Controller
    {
        
        public HomeController()
        {
            
        }
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}