using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SkillsLab2023_Assignment.Models
{
    public class EnrollViewModel
    {
        [Required(ErrorMessage = "Training Id is required")]
        public short TrainingId { get; set; }

        [Required(ErrorMessage = "Training Name is required")]
        public string TrainingName { get; set; }
        public List<DocumentUploadViewModel> Files { get; set; }
    }
}