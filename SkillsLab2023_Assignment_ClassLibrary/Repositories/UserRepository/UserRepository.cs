using SkillsLab2023_Assignment_ClassLibrary.Repositories.DatabaseCommand;


namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly IDatabaseCommand _dbCommand;
        public UserRepository(IDatabaseCommand dbCommand) 
        {
            _dbCommand = dbCommand;
        }
    }
}
