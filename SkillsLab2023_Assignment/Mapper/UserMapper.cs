using DAL.Models;
using SkillsLab2023_Assignment.Models;

namespace SkillsLab2023_Assignment.Mapper
{
    public static class UserMapper
    {
        public static User ToUser(this RegisterViewModel registerViewModel)
        {
            return new User
            {
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                MobileNumber = registerViewModel.MobileNumber,
                NationalIdentityCard = registerViewModel.NationalIdentityCard,
                DepartmentId = registerViewModel.DepartmentId,
                ManagerId = registerViewModel.ManagerId,
                Email = registerViewModel.Email
            };
        }
    }
}