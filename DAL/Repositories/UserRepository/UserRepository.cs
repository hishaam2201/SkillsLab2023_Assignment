using DAL.DatabaseCommand.DatabaseCommand;
using DAL.Models;


namespace DAL.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly IDatabaseCommand<User> _dbCommand;
        public UserRepository(IDatabaseCommand<User> dbCommand) 
        {
            _dbCommand = dbCommand;
        }
    }
}
