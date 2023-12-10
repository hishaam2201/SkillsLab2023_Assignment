

using SkillsLab2023_Assignment_ClassLibrary.KeyAttribute;

namespace SkillsLab2023_Assignment_ClassLibrary.Entity
{
    public class Account
    {
        [PrimaryKey]
        public int Id { get; private set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [ForeignKey]
        public int UserId { get; private set; }
        
    }
}
