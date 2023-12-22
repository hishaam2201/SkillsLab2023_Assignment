using DAL.DTO;
using DAL.Models;
using DAL.Repositories.ApplicationRepository;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Services.ApplicationService
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        public ApplicationService(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<bool> ProcessApplication(List<DocumentUploadDTO> documentUploads)
        {
            if (documentUploads != null && documentUploads.Any())
            {
                foreach (var documentUpload in documentUploads)
                {
                    if (documentUpload.File != null && documentUpload.File.ContentLength > 0)
                    {
                        // Read file content into a byte array
                        byte[] fileData;
                        using (var  stream = new MemoryStream())
                        {
                            await documentUpload.File.InputStream.CopyToAsync(stream);
                            fileData = stream.ToArray();
                        }

                        var documentUploadEntity = new DocumentUpload
                        {
                            File = fileData,
                            PreRequisiteId = documentUpload.PreRequisiteId
                        };
                        var applicationEntity = new Application
                        {
                            UserId = documentUpload.UsertId,
                            TrainingId = documentUpload.TrainingId
                        };

                        // Call ApplyForTraining
                        if (!await _applicationRepository.ApplyForTraining(applicationEntity, documentUploadEntity))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
