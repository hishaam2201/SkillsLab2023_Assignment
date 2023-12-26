

namespace DAL.Models
{
    public class DocumentUpload
    {
        public int Id { get; private set; }
        public int ApplicationId { get; set; }
        public byte[] File { get; set; }
        public int PreRequisiteId { get; set; }
        public string FileName { get; set; }
    }
}
