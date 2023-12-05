using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.DatabaseCommand;


namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.UserRepository
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
