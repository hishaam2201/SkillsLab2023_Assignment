

namespace DAL.Models
{
    public class User
    {
        public int Id { get; private set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int MobileNumber { get; set; }
        public string NationalIdentityCard { get; set; }
        public int DepartmentId { get; set; }
        public int RoleId { get; private set; }
        public int ManagerId { get; set; }
    }
}
