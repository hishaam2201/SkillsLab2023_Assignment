using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SkillsLab2023_Assignment.Models
{
    public class RegisterViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string NationalIdentityCard { get; set; }
        public byte DepartmentId { get; set; }
        public byte ManagerId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}