

namespace DAL.DTO
{
    public class ApplicationDTO
    {
        public int ApplicationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string TrainingName { get; set; }
        public string DepartmentName { get; set; }
        public string ApplicationStatus { get; set; }
        public string DeclineReason { get; set; }
    }
}
