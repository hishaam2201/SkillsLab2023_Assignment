using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.DatabaseCommand;


namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.ApplicationRepository
{
    public class ApplicationRepository : IApplicationRepository
    {
        private IDatabaseCommand<Application> _dbCommand;
        public ApplicationRepository(IDatabaseCommand<Application> dbCommand) 
        {
            _dbCommand = dbCommand;
        }
    }

}
