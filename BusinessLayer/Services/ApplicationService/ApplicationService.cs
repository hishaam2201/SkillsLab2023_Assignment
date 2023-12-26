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
                var firstDocument = documentUploads.First();
                var applicationEntity = new Application
                {
                    UserId = firstDocument.UsertId,
                    TrainingId = firstDocument.TrainingId,
                };

                int applicationId = await _applicationRepository.InsertApplicationAndGetId(applicationEntity);
                if (applicationId <= 0)
                {
                    return false;
                }

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
                            ApplicationId = applicationId,
                            File = fileData,
                            PreRequisiteId = documentUpload.PreRequisiteId,
                            FileName = documentUpload.FileName
                        };
                        
                        if (!await _applicationRepository.InsertDocumentUpload(documentUploadEntity))
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
