namespace DAL.DTO
{
    public class ApplicationDocumentDTO
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public AttachmentInfoDTO AttachmentInfoDTO { get; set; }
    }
}
