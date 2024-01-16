using System;


namespace DAL.Models
{
    public class Application
    {
        public int Id { get; set; }
        public string ApplicationStatus { get; set; }
        public DateTime ApplicationDateTime { get; set; }
        public short UserId { get; set; }
        public short TrainingId { get; set; }
        public string DeclineReason { get; set; }
        public DateTime? SelectedDate { get; set; }
    }
}
