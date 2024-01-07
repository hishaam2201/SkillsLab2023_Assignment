using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SkillsLab2023_Assignment.Models
{
    public class EditTrainingViewModel : TrainingViewModel
    {
        [Required]
        public int TrainingId { get; set; }

    }
}