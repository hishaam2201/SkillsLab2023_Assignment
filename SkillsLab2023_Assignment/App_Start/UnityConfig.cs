
using SkillsLab2023_Assignment.CustomExceptionHandler.AppLogger;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.AccountRepository;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.DAL;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.DatabaseCommand;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.TrainingRepository;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.UserRepository;
using SkillsLab2023_Assignment_ClassLibrary.Services.AccountService;
using SkillsLab2023_Assignment_ClassLibrary.Services.TrainingService;
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
            container.RegisterType(typeof(IDatabaseCommand<>), typeof(DatabaseCommand<>));
            container.RegisterType<ILogger, Logger>();


            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IUserService, UserService>();

            container.RegisterType<IAccountRepository, AccountRepository>();
            container.RegisterType<IAccountService, AccountService>();

            container.RegisterType<ITrainingRepository, TrainingRepository>();
            container.RegisterType<ITrainingService, TrainingService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}