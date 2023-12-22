
using System.Web;

namespace SkillsLab2023_Assignment.Models
{
    public class DocumentUploadViewModel
    {
        public int TrainingId { get; set; }
        public int PreRequisiteId { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}