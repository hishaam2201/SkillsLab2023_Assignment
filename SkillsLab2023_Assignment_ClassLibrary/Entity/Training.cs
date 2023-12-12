using SkillsLab2023_Assignment_ClassLibrary.KeyAttribute;
using System;

namespace SkillsLab2023_Assignment_ClassLibrary.Entity
{
    public class Training
    {
        [PrimaryKey]
        public int Id { get; private set; }
        public string Description { get; set; }
        public string ProgrammeName { get; set; }
        public int Capacity { get; set; }
        public DateTime StartingDate { get; set; }
        public DateTime Deadline { get; set; }
        [ForeignKey]
        public int DepartmentId { get; set; }
    }
}
