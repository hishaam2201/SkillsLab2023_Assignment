
using System.Web;

namespace DAL.DTO
{
    public class DocumentUploadDTO
    {
        public short UsertId { get; set; }
        public int TrainingId { get; set; }
        public int PreRequisiteId { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}
