using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace SkillsLab2023_Assignment.Custom
{
    public class FileExtensionAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions;

        public FileExtensionAttribute(params string[] allowedExtensions)
        {
            _allowedExtensions = allowedExtensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is HttpPostedFileBase file)
            {
                if(!IsFileExtensionValid(file.FileName))
                {
                    return new ValidationResult("Invalid file extension");
                }
            }
            return ValidationResult.Success;
        }

        private bool IsFileExtensionValid(string filename)
        {
            var extension = Path.GetExtension(filename).ToLowerInvariant();
            return _allowedExtensions.Any(ext => ext.ToLowerInvariant() == extension);
        }
    }
}