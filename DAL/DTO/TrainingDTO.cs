
using System;
using System.Collections.Generic;

namespace DAL.DTO
{
    public class TrainingDTO
    {
        public short TrainingId { get; set; }
        public string TrainingName { get; set; }
        public string Description { get; set; }
        public DateTime TrainingCourseStartingDateTime { get; set; }
        public DateTime DeadlineOfApplication { get; set; }
        public int Capacity { get; set; }
        public string DepartmentName { get; set; }
        public bool IsDeadlineExpired { get; set; }
        public List<TrainingPreRequisteDTO> PreRequisites { get; set; }
    }
}
