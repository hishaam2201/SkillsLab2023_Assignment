using DAL.Models;


namespace BusinessLayer.Services.AccountService
{
    public interface IAccountService
    {
        bool AuthenticateLoginCredentials(string email, string password);
        bool EmailExists(string email);
        bool Register(User user);
    }
}
