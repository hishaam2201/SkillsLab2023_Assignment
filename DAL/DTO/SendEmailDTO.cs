

using System;

namespace DAL.DTO
{
    public class SendEmailDTO
    {
        public string EmployeeName { get; set; }
        public string ManagerName { get; set; }
        public string EmployeeEmail { get; set; }
        public string TrainingName { get; set; }
        public DateTime TrainingStartDate { get; set; }

    }
}
