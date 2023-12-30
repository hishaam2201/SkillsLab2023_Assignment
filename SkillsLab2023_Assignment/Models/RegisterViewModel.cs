using System.ComponentModel.DataAnnotations;


namespace SkillsLab2023_Assignment.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "FirstName is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName is required.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "MobileNumber is required.")]
        [StringLength(8, ErrorMessage = "MobileNumber must be exactly 8 characters.", MinimumLength = 8)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "MobileNumber must contain only numeric characters.")]
        public string MobileNumber { get; set; }
        [Required]
        [StringLength(14, ErrorMessage = "NIC must be exactly 14 characters.", MinimumLength = 14)]
        [RegularExpression("^[A-Z][0-9]{12}[A-Z]$", ErrorMessage = "Invalid NIC format.")]
        public string NationalIdentityCard { get; set; }
        [Required(ErrorMessage = "Department is required.")]
        public byte DepartmentId { get; set; }
        [Required(ErrorMessage = "Manager is required.")]
        public byte ManagerId { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression("^[\\w\\.-]+@[a-zA-Z\\d\\.-]+\\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Format.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")] // TODO: Validation
        public string Password { get; set; }
    }
}