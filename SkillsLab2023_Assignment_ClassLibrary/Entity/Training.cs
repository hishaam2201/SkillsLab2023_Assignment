using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Entity
{
    public class Training
    {
        public int TrainingId { get; private set; }
        public string ProgrammeName { get; set; }
        public int Capacity { get; set; }
        public DateTime StartingDate { get; set; }
        public DateTime Deadline { get; set; }
        public int DepartmentId { get; set; }
    }
}
