using DAL.DatabaseCommand.DatabaseCommand;
using DAL.Models;

namespace DAL.Repositories.ApplicationRepository
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly IDatabaseCommand<Application> _dbCommand;
        public ApplicationRepository(IDatabaseCommand<Application> dbCommand) 
        {
            _dbCommand = dbCommand;
        }
    }

}
