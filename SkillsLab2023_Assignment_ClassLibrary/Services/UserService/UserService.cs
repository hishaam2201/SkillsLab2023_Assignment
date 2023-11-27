using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.GenericRepository;
using SkillsLab2023_Assignment_ClassLibrary.Services.GenericService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Services.UserService
{
    public class UserService : GenericService<User>, IUserService
    {
        public UserService(IGenericRepository<User> repository) : base(repository)
        {

        }
    }
}
