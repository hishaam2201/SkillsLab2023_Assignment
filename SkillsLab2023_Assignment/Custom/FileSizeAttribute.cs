using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SkillsLab2023_Assignment.Custom
{
    public class FileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;

        public FileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is HttpPostedFileBase file)
            {
                if (file.ContentLength > _maxFileSize)
                {
                    return new ValidationResult($"File size cannot exceed {_maxFileSize / (1024 * 1024)} MB");
                }
            }
            return ValidationResult.Success;
        }
    }
}