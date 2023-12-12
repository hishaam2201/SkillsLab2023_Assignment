using BusinessLayer.Services.AccountService;
using BusinessLayer.Services.TrainingService;
using BusinessLayer.Services.UserService;
using Framework.DatabaseCommand.DatabaseCommand;
using DAL.Repositories.AccountRepository;
using DAL.Repositories.TrainingRepository;
using DAL.Repositories.UserRepository;
using Framework.AppLogger;
using Framework.DAL;
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