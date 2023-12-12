using System;


namespace DAL.Models
{
    public class Application
    {
        public int Id { get; private set; }
        public int UserId { get; set; } 
        public int TrainingId { get; set; } 
        public string ApplicationStatus { get; set; }
        public DateTime ApplicationDateTime { get; set; }
    }
}
