using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.GenericRepository;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.UserRepository;
using SkillsLab2023_Assignment_ClassLibrary.Services.GenericService;
using SkillsLab2023_Assignment_ClassLibrary.Services.UserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Services.AccountService
{
    public class AccountService : GenericService<Account>, IAccountService
    {
        private readonly IUserRepository _userRepository;

        public AccountService(IGenericRepository<Account> repository, IUserRepository userRepository) : base(repository)
        {
            _userRepository = userRepository;
        }

        public bool Login(Account account)
        {
            throw new NotImplementedException();
        }

        public bool Register(User user)
        {
            // Example
            string message;
            message = "jwbhk";
            return _userRepository.Create(user, out message);
        }
    }
}
