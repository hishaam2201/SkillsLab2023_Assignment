using System;

namespace DAL.Models
{
    public class Training
    {
        public int Id { get; private set; }
        public string Description { get; set; }
        public string ProgrammeName { get; set; }
        public int Capacity { get; set; }
        public DateTime StartingDate { get; set; }
        public DateTime Deadline { get; set; }
        public int DepartmentId { get; set; }
    }
}
