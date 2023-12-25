using Framework.Enums;

namespace Framework.StaticClass
{
    public static class Extensions
    {
        public static string GetDashboardAction(this byte roleId)
        {
            switch ((RoleEnum)roleId)
            {
                case RoleEnum.Employee:
                    return "EmployeeDashboard";
                case RoleEnum.Manager:
                    return "ManagerDashboard";
                case RoleEnum.Administrator:
                    return "AdministratorDashboard";
                default:
                    return "AccessDenied";
            }
        }
    }
}
