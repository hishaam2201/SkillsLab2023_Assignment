using SkillsLab2023_Assignment_ClassLibrary.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Services.TrainingService
{
    public interface ITrainingService
    {
        IEnumerable<Training> GetAllTrainings();
    }
}
