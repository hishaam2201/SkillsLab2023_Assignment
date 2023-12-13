using Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.StaticClass
{
    public static class Extensions
    {
        public static string RemoveUnderScores(this DepartmentEnum value)
        {
            return value.ToString().Replace('_', ' ');
        }

        public static string GetDashboardAction(this int roleId)
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
