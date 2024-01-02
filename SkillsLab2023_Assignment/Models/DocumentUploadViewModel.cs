
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SkillsLab2023_Assignment.Models
{
    public class DocumentUploadViewModel
    {
        [Required(ErrorMessage="Training is required")]
        public int TrainingId { get; set; }
        [Required(ErrorMessage = "PreRequisite is required")]
        public int PreRequisiteId { get; set; }
        [Required(ErrorMessage = "File is required")]
        public HttpPostedFileBase File { get; set; }
        [Required(ErrorMessage = "FileName is required")]
        public string FileName { get; set; }
    }
}