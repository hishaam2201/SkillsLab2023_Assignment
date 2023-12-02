using SkillsLab2023_Assignment_ClassLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Entity
{
    public class Application
    {
        public int ApplicationId { get; private set; }
        public int UserId { get; set; } 
        public int TrainingId { get; set; } 
        public string ApplicationStatus { get; set; }
        public DateTime ApplicationDateTime { get; set; }
    }
}
