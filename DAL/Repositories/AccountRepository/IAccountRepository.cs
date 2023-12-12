using DAL.Models;


namespace DAL.Repositories.AccountRepository
{
    public interface IAccountRepository
    {
        bool AuthenticateLoginCredentials(string email, string password);
        bool EmailExists(string email);
        int GetRoleId(string role);
        bool Register(User user);
    }
}
