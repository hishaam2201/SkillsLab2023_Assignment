

namespace DAL.DTO
{
    public class ApplicationDocumentDTO
    {
        public int AttachmentId { get; set; }
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public string PreRequisiteName { get; set; }
        public string PreRequisiteDescription { get; set; }
    }
}
