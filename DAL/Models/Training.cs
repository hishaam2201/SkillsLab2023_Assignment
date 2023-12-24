﻿using System;

namespace DAL.Models
{
    public class Training
    {
        public int Id { get; private set; }
        public string TrainingName { get; set; }
        public string Description { get; set; }
        public DateTime TrainingCourseStartingDate { get; set; }
        public DateTime DeadlineOfApplication { get; set; }
        public int Capacity { get; set; }
        public int DepartmentId { get; set; }
    }
}
