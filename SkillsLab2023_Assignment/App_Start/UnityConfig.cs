
using SkillsLab2023_Assignment_ClassLibrary.Repositories.AccountRepository;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.DataAccessLayer;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.UserRepository;
using SkillsLab2023_Assignment_ClassLibrary.Services.AccountService;
using SkillsLab2023_Assignment_ClassLibrary.Services.UserService;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace SkillsLab2023_Assignment
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            
            container.RegisterType<IDataAccessLayer, DataAccessLayer>();

            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IUserService, UserService>();

            container.RegisterType<IAccountRepository, AccountRepository>();
            container.RegisterType<IAccountService, AccountService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}