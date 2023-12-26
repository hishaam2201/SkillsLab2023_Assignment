using DAL.DTO;
using System.Collections.Generic;
using System.Web;

namespace SkillsLab2023_Assignment.Custom
{
    public class SessionManager
    {
        private const string _currentUserKey = "CurrentUser";
        private const string _userRoleKey = "UserRole";
        private const string _emailKey = "Email";
        private const string _attachments = "Attachments";

        public static UserDTO CurrentUser
        {
            get => Get<UserDTO>(_currentUserKey);
            set => Set(_currentUserKey, value);
        }

        public static string UserRole
        {
            get => Get<string>(_userRoleKey);
            set => Set(_userRoleKey, value);
        }

        public static string Email
        {
            get => Get<string>(_emailKey);
            set => Set(_emailKey, value);
        }

        public static List<PendingApplicationDocumentDTO> Attachments
        {
            get => Get<List<PendingApplicationDocumentDTO>>(_attachments);
            set => Set(_attachments, value);
        }

        private static T Get<T>(string key)
        {
            if (HttpContext.Current.Session[key] != null)
            {
                return (T)HttpContext.Current.Session[key];
            }
            return default;
        }

        private static void Set(string key, object value)
        {
            HttpContext.Current.Session[key] = value;
        }

        public static void Remove(string key)
        {
            HttpContext.Current.Session.Remove(key);
        }

        public static void ClearSession()
        {
            HttpContext.Current.Session.Clear();
        }
    }
}