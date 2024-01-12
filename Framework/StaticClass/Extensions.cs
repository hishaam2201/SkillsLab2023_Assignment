using Framework.Enums;
using System.IO;

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

        public static string GetContentTypeFromFileName(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();

            switch (extension)
            {
                case ".pdf":
                    return "application/pdf";
                case ".jpeg":
                case ".jpg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
