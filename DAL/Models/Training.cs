﻿using System;

namespace DAL.Models
{
    public class Training
    {
        public short Id { get; set; }
        public string TrainingName { get; set; }
        public string Description { get; set; }
        public DateTime TrainingCourseStartingDateTime { get; set; }
        public DateTime DeadlineOfApplication { get; set; }
        public int Capacity { get; set; }
        public byte DepartmentId { get; set; }
        public bool IsDeadlineExpired { get; set; }
    }
}
