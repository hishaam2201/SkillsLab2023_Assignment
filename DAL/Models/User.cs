

namespace DAL.Models
{
    public class User
    {
        public short Id { get; private set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string NationalIdentityCard { get; set; }
        public byte DepartmentId { get; set; }
        public byte ManagerId { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
    }
}
