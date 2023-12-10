﻿using SkillsLab2023_Assignment_ClassLibrary.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.DTO
{
    public class TrainingDTO
    {
        public int TrainingId { get; set; }
        public string Description { get; set; }
        public string ProgrammeName { get; set; }
        public int Capacity { get; set; }
        public DateTime StartingDate { get; set; }
        public DateTime Deadline { get; set; }
        public string DepartmentName { get; set; }
        public List<PreRequiste> PreRequistes { get; set; }
    }
}
