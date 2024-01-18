using System.ComponentModel.DataAnnotations;

namespace SkillsLab2023_Assignment.Models
{
    public class EditTrainingViewModel : TrainingViewModel
    {
        [Required(ErrorMessage = "TrainingId is required")]
        public short TrainingId { get; set; }

    }
}