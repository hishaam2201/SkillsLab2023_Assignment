

namespace DAL.DTO
{
    public class RegistrationDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int MobileNumber { get; set; }
        public string NationalIdentityCard { get; set; }
        public byte DepartmentId { get; set; }
        public byte ManagerId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
