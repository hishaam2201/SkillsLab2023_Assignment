using BusinessLayer.Services.EnrollmentProcessService;
using BusinessLayer.Services.TrainingService;
using Hangfire;
using SkillsLab2023_Assignment.App_Start;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SkillsLab2023_Assignment
{
    public class MvcApplication : HttpApplication
    {
        private const string HANGFIRE_DB_CONNECTION = "HangFireDbConnection";
        private IEnumerable<IDisposable> GetHangfireServers()
        {
            var connectionString = ConfigurationManager.AppSettings[HANGFIRE_DB_CONNECTION];

            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString)
                .UseActivator(new ContainerJobActivator(UnityConfig.Container));

            yield return new BackgroundJobServer();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            UnityConfig.RegisterComponents();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            HangfireAspNet.Use(GetHangfireServers);

            // Schedule update deadline expiry status job
            BackgroundJob.Schedule<ITrainingService>
                (trainingService => trainingService.PerformAutomaticDeadlineExpiryStatusUpdateAsync(), TimeSpan.FromHours(24));
            // Schedule selection process
            BackgroundJob.Schedule<IEnrollmentProcessService>
                (enrollmentProcessService => enrollmentProcessService.PerformAutomaticSelectionProcessAsync(), TimeSpan.FromHours(24));
        }

        protected void Application_BeginRequest()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
        }
    }
}
