using Hangfire;
using System;
using Unity;

namespace SkillsLab2023_Assignment.App_Start
{
    public class ContainerJobActivator : JobActivator
    {
        private readonly IUnityContainer _container;

        public ContainerJobActivator(IUnityContainer container)
        {
            _container = container;
        }

        public override object ActivateJob(Type type)
        {
            return _container.Resolve(type);
        }
    }
}