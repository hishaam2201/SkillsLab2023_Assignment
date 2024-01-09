

using Framework.Enums;
using System;

namespace DAL.DTO
{
    public class UserApplicationDTO
    {
        public ApplicationStatusEnum ApplicationStatus { get; set; }
        public string TrainingName { get; set; }
        public string TrainingDepartment { get; set; }
        public string DeclineReason { get; set; } // By default nullable
        public DateTime ApplicationDateTime { get; set; }
    }
}
