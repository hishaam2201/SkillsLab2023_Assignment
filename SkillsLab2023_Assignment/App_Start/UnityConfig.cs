
using SkillsLab2023_Assignment_ClassLibrary.Repositories.DataAccessLayer;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.GenericRepository;
using SkillsLab2023_Assignment_ClassLibrary.Services.GenericService;
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
            container.RegisterType(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            container.RegisterType(typeof(IGenericService<>), typeof(GenericService<>));
            container.RegisterType<IUserService, UserService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}