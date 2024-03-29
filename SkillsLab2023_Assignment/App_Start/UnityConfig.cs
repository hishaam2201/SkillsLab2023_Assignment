using BusinessLayer.Services.TrainingService;
using Framework.DatabaseCommand.DatabaseCommand;
using DAL.Repositories.UserRepository;
using DAL.Repositories.TrainingRepository;
using Framework.AppLogger;
using Framework.DAL;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using BusinessLayer.Services.ApplicationService;
using DAL.Repositories.ApplicationRepository;
using BusinessLayer.Services.EnrollmentProcessService;
using DAL.Repositories.EnrollmentProcessRepository;
using Framework.BackgroundEnrollmentProcessLogger;
using BusinessLayer.Services.UserService;

namespace SkillsLab2023_Assignment
{
    public static class UnityConfig
    {
        public static IUnityContainer Container { get; internal set; }
        public static void RegisterComponents()
        {
            Container = new UnityContainer();

            Container.RegisterType<ILogger, Logger>();
            Container.RegisterType<IBackgroundJobLogger, BackgroundJobLogger>();

            Container.RegisterType<IDataAccessLayer, DataAccessLayer>();
            Container.RegisterType(typeof(IDatabaseCommand<>), typeof(DatabaseCommand<>));

            Container.RegisterType<IEnrollmentProcessService, EnrollmentProcessService>();
            Container.RegisterType<IEnrollmentProcessRepository, EnrollmentProcessRepository>();

            Container.RegisterType<IUserService, UserService>();
            Container.RegisterType<IUserRepository, UserRepository>();

            Container.RegisterType<ITrainingService, TrainingService>();
            Container.RegisterType<ITrainingRepository, TrainingRepository>();

            Container.RegisterType<IApplicationService, ApplicationService>();
            Container.RegisterType<IApplicationRepository, ApplicationRepository>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(Container));
        }
    }
}