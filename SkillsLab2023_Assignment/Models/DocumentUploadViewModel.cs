using SkillsLab2023_Assignment.Custom;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SkillsLab2023_Assignment.Models
{
    public class DocumentUploadViewModel
    {
        [Required(ErrorMessage = "PreRequisite is required")]
        public int PreRequisiteId { get; set; }

        [Required(ErrorMessage = "File is required")]
        [FileSize(6* 1024 * 1024, ErrorMessage = "Maximum allowed file size is 6 MB")]
        [FileExtension(".pdf", ".jpeg", ".png", ErrorMessage = "Invalid file extension")]
        public HttpPostedFileBase File { get; set; }

        [Required(ErrorMessage = "FileName is required")]
        public string FileName { get; set; }
    }
}