using SkillsLab2023_Assignment_ClassLibrary.KeyAttribute;
using System;
using System.ComponentModel.DataAnnotations;

namespace SkillsLab2023_Assignment_ClassLibrary.Entity
{
    public class Training
    {
        [PrimaryKey]
        public int TrainingId { get; private set; }
        public string ProgrammeName { get; set; }
        public int Capacity { get; set; }
        public DateTime StartingDate { get; set; }
        public DateTime Deadline { get; set; }
        [ForeignKey]
        public int DepartmentId { get; set; }
        public string DepartmentName {  get; set; } = string.Empty;
    }
}
