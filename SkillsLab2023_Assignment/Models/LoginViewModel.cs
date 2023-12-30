

using System.ComponentModel.DataAnnotations;

namespace SkillsLab2023_Assignment.Models
{
    public class LoginViewModel
    {
        [Required]
        [RegularExpression("^[\\w\\.-]+@[a-zA-Z\\d\\.-]+\\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Format.")]
        public string Email { get; set; }
        [Required] // TODO: Validate password
        public string Password { get; set; }
    }
}