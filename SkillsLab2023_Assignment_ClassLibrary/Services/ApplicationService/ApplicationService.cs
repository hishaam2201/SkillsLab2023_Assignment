using SkillsLab2023_Assignment_ClassLibrary.Repositories.ApplicationRepository;


namespace SkillsLab2023_Assignment_ClassLibrary.Services.ApplicationService
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        public ApplicationService(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }
    }
}
