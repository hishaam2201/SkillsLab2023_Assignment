using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Entity
{
    public class User
    {
        public int Id { get; private set; }
        public int DepartmentId { get; set; } // FK
        public int RoleId { get; private set; } // FK
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int MobileNumber { get; set; }
        public string NIC { get; set; }
        public string ManagerName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
