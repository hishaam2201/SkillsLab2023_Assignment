using System;
using System.Collections.Generic;

namespace DAL.DTO
{
    public class SelectedProcessUserDTO
    {
        public string TrainingName { get; set; }
        public string TrainingDepartment { get; set; }
        public DateTime TrainingStartDateTime { get; set; }
        public List<SelectionProcessDTO> SelectedProcessUsers { get; set; }
    }
}
