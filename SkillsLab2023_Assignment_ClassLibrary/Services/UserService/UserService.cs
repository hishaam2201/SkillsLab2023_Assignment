using SkillsLab2023_Assignment_ClassLibrary.Repositories.UserRepository;


namespace SkillsLab2023_Assignment_ClassLibrary.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
    }
}
