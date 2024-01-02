using System;


namespace DAL.Models
{
    public class Application
    {
        public int Id { get; private set; }
        public string ApplicationStatus { get; private set; }
        public DateTime ApplicationDateTime { get; private set; }
        public short UserId { get; set; }
        public int TrainingId { get; set; }
        public string DeclineReason { get; set; }
    }
}
