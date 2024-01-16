
using System;
using System.ComponentModel.DataAnnotations;

namespace SkillsLab2023_Assignment.Models
{
    public class TrainingViewModel
    {
        [Required]
        public string TrainingName { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime ApplicationDeadline { get; set; }
        [Required]
        public int Capacity { get; set; }
        [Required]
        public byte DepartmentId { get; set; }
        [Required]
        public DateTime TrainingStartDateTime { get; set; }
        public string PreRequisiteIds { get; set; }
    }
}