

namespace DAL.Models
{
    public class User
    {
        public int Id { get; private set; }
        public int RoleId { get; private set; }
        public int DepartmentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int MobileNumber { get; set; }
        public string NationalIdentityCard { get; set; }
        public string ManagerName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
