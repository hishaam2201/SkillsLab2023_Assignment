using DAL.Models;
using SkillsLab2023_Assignment.Models;
using System;

namespace SkillsLab2023_Assignment.Mapper
{
    public static class TrainingMapper
    {
        public static Training ToTraining(this EditTrainingViewModel editTrainingViewModel)
        {
            return new Training()
            {
                Id = (short)editTrainingViewModel.TrainingId,
                TrainingName = editTrainingViewModel.TrainingName,
                Description = editTrainingViewModel.Description,
                TrainingCourseStartingDateTime = editTrainingViewModel.TrainingStartDateTime,
                DeadlineOfApplication = editTrainingViewModel.ApplicationDeadline,
                Capacity = editTrainingViewModel.Capacity,
                DepartmentId = (byte)editTrainingViewModel.DepartmentId,
                IsDeadlineExpired = editTrainingViewModel.ApplicationDeadline < DateTime.Now
            };
        }

        public static Training ToTraining(this TrainingViewModel trainingViewModel)
        {
            return new Training()
            {
                TrainingName = trainingViewModel.TrainingName,
                Description = trainingViewModel.Description,
                TrainingCourseStartingDateTime = trainingViewModel.TrainingStartDateTime,
                DeadlineOfApplication = trainingViewModel.ApplicationDeadline,
                Capacity = trainingViewModel.Capacity,
                DepartmentId = (byte)trainingViewModel.DepartmentId,
                IsDeadlineExpired = trainingViewModel.ApplicationDeadline < DateTime.Now
            };
        }
    }
}