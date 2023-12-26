using BusinessLayer.Services.AccountService;
using BusinessLayer.Services.TrainingService;
using Framework.DatabaseCommand.DatabaseCommand;
using DAL.Repositories.AccountRepository;
using DAL.Repositories.TrainingRepository;
using Framework.AppLogger;
using Framework.DAL;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using BusinessLayer.Services.ApplicationService;
using DAL.Repositories.ApplicationRepository;
using BusinessLayer.Services.AdministratorService;
using DAL.Repositories.AdministratorRepository;
using BusinessLayer.Services.EnrollmentProcessService;
using DAL.Repositories.EnrollmentProcessRepository;

namespace SkillsLab2023_Assignment
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            container.RegisterType<ILogger, Logger>();
            container.RegisterType<IDataAccessLayer, DataAccessLayer>();
            container.RegisterType(typeof(IDatabaseCommand<>), typeof(DatabaseCommand<>));

            container.RegisterType<IEnrollmentProcessService, EnrollmentProcessService>();
            container.RegisterType<IEnrollmentProcessRepository, EnrollmentProcessRepository>();

            container.RegisterType<IAdministratorService, AdministratorService>();
            container.RegisterType<IAdministratorRepository, AdministratorRepository>();

            container.RegisterType<IAccountService, AccountService>();
            container.RegisterType<IAccountRepository, AccountRepository>();

            container.RegisterType<ITrainingService, TrainingService>();
            container.RegisterType<ITrainingRepository, TrainingRepository>();

            container.RegisterType<IApplicationService, ApplicationService>();
            container.RegisterType<IApplicationRepository, ApplicationRepository>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}