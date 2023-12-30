
using System.Collections.Generic;

namespace DAL.DTO
{
    public class TrainingDTO
    {
        public int TrainingId { get; set; }
        public string TrainingName { get; set; }
        public string Description { get; set; }
        public string TrainingCourseStartingDateTime { get; set; }
        public string DeadlineOfApplication { get; set; }
        public int Capacity { get; set; }
        public string DepartmentName { get; set; }
        public bool IsDeadlineExpired { get; set; }
        public List<TrainingPreRequisteDTO> PreRequisites { get; set; }
    }
}
