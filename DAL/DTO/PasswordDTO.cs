

namespace DAL.DTO
{
    public class PasswordDTO
    {
        public byte[] HashedPassword { get; set; }
        public byte[] Salt {  get; set; }
    }
}
