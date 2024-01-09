

using System;

namespace DAL.DTO
{
    public class UserDTO
    {
        public short Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte? DepartmentId { get; set; }
        public short? ManagerId { get; set; }
        public string Email { get; set; }
        public byte RoleId { get; set; }
        public string DepartmentName { get; set; }
        public DateTime ApplicationDateTime { get; set; }
    }
}
