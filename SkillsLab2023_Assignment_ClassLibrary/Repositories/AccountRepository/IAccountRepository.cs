using SkillsLab2023_Assignment_ClassLibrary.Entity;


namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.AccountRepository
{
    public interface IAccountRepository
    {
        bool AuthenticateLoginCredentials(string email, string password);
        bool EmailExists(string email);
        int GetRoleId(string role);
        bool Register(User user);
    }
}
