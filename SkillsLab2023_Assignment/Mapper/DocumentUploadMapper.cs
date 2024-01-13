using DAL.DTO;
using SkillsLab2023_Assignment.Models;
using System.Collections.Generic;
using System.Linq;

namespace SkillsLab2023_Assignment.Mapper
{
    public static class DocumentUploadMapper
    {
        public static List<DocumentUploadDTO> ToDocumentUploadWithPreRequisites(this List<DocumentUploadViewModel> files, short trainingId, string trainingName)
        {
            return files.Select(file => new DocumentUploadDTO
            {
                TrainingId = trainingId,
                TrainingName = trainingName,
                PreRequisiteId = file.PreRequisiteId,
                File = file.File,
                FileName = file.FileName
            }).ToList();
        }
    }
}