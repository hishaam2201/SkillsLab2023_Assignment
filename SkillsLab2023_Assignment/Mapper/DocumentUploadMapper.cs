using DAL.DTO;
using SkillsLab2023_Assignment.Models;
using System.Collections.Generic;
using System.Linq;

namespace SkillsLab2023_Assignment.Mapper
{
    public static class DocumentUploadMapper
    {
        public static List<DocumentUploadDTO> ToEnrollmentDataWithPreRequisites(this List<DocumentUploadViewModel> files, short userId, short trainingId)
        {
            return files.Select(file => new DocumentUploadDTO
            {
                UsertId = userId,
                TrainingId = trainingId,
                PreRequisiteId = file.PreRequisiteId,
                File = file.File,
                FileName = file.FileName
            }).ToList();
        }
    }
}