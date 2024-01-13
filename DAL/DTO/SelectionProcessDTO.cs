using System;
using System.Collections.Generic;

namespace DAL.DTO
{
    public class SelectionProcessDTO
    {
        public int TrainingId { get; set; }
        public string TrainingName { get; set; }
        public string TrainingDepartment { get; set; }
        public DateTime TrainingStartDateTime { get; set; }
        public List<SelectedUserDTO> SelectedUsersList { get; set; }
    }
}
