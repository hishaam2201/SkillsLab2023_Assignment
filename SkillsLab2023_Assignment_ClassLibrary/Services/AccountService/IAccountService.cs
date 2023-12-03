using SkillsLab2023_Assignment_ClassLibrary.Entity;


namespace SkillsLab2023_Assignment_ClassLibrary.Services.AccountService
{
    public interface IAccountService
    {
        bool AuthenticateLoginCredentials(string email, string password);
        bool EmailExists(string email);
        bool Register(User user);
    }
}
