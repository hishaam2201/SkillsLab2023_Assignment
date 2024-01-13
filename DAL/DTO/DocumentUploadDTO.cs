using System.Web;

namespace DAL.DTO
{
    public class DocumentUploadDTO
    {
        public short UsertId { get; set; }
        public short TrainingId { get; set; }
        public string TrainingName { get; set; }
        public int PreRequisiteId { get; set; }
        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
    }
}
