﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class PendingApplicationDocumentDTO
    {
        public int ApplicationId { get; set; }
        public byte[] File { get; set; }
        public string PreRequisiteDescription { get; set; }
    }
}