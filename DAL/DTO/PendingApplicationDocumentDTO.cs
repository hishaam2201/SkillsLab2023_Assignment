﻿

namespace DAL.DTO
{
    public class PendingApplicationDocumentDTO
    {
        public int AttachmentId { get; set; }
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public string PreRequisiteDescription { get; set; }
    }
}
