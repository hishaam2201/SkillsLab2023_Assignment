using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SkillsLab2023_Assignment.Models
{
    public class EditTrainingViewModel : TrainingViewModel
    {
        [Required(ErrorMessage = "TrainingId is required")]
        public int TrainingId { get; set; }

    }
}