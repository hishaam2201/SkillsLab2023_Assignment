
using Framework.Enums;
using System;

namespace DAL.DTO
{
    public class SelectionProcessDTO
    {
        public short UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string TrainingName { get; set; }
        public DateTime TrainingCourseStartingDateTime { get; set; }
        public ApplicationStatusEnum ApplicationStatus { get; set; }
        public string DepartmentName { get; set; }
    }
}
