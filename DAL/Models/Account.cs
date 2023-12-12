

namespace DAL.Models
{
    public class Account
    {
        public int Id { get; private set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int UserId { get; private set; }
    }
}
