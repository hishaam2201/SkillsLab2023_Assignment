using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Entity
{
    public class Account
    {
        public int Id { get; private set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int UserId { get; private set; }
    }
}
