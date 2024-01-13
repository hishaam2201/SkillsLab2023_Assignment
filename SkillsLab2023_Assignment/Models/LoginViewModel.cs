

using System.ComponentModel.DataAnnotations;

namespace SkillsLab2023_Assignment.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression("^[\\w\\.-]+@[a-zA-Z\\d\\.-]+\\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Format.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}