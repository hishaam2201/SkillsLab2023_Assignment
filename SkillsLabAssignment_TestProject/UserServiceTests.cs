using Moq;
using DAL.Models;
using NUnit.Framework;
using System.Collections.Generic;
using DAL.Repositories.UserRepository;
using BusinessLayer.Services.UserService;
using System.Threading.Tasks;
using DAL.DTO;
using Framework.HelperClasses;
using System.Linq;
using Framework.Enums;

namespace SkillsLabAssignment_TestProject
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _stubUserRepository;
        private UserService _userService;

        private List<User> _userRepository;
        private List<DepartmentDTO> _departmentRepository;
        private List<UserRole> _userRoleRepository;

        [SetUp]
        public void Setup()
        {
            // Items already "present" in database
            _userRepository = new List<User>
            {
                CreateUser(1, "Junaid", "Edoo", "58394875", "H738475619273M", null, null, "junaid.edoo@gmail.com", "password2"),
                CreateUser(2, "Divesh", "Nugessur", "59173649", "D771937452281N", 2, null, "divesh@gmail.com", "password1"),
                CreateUser(3, "John", "Doe", "58328374", "J456378976532D", 1, 6, "john.doe@gmail.com", "password"),
                CreateUser(4, "Rushmee", "Toolsee", "58936499", "R001936481944T", 2, 2, "rushmee@gmail.com", "password3"),
                CreateUser(5, "Haniyyah", "Munsoor", "59378347", "H675376574423M", 3, 5, "haniyyahmunsoor6@gmail.com", "haniyyah"),
                CreateUser(6, "Hello", "World", "59111347", "H675222574423M", 1, null, "hello.world@gmail.com", "helloworld")
            };
            _departmentRepository = new List<DepartmentDTO>
            {
                new DepartmentDTO { Id = 1, DepartmentName = "Product and Technology"},
                new DepartmentDTO { Id = 2, DepartmentName = "Customer Support"},
                new DepartmentDTO { Id = 3, DepartmentName = "Finance"},
                new DepartmentDTO { Id = 4, DepartmentName = "Payroll"},
                new DepartmentDTO { Id = 5, DepartmentName = "Services"}
            };
            _userRoleRepository = new List<UserRole>
            {
                new UserRole { UserId = 1, RoleId = RoleEnum.Administrator },
                new UserRole { UserId = 2, RoleId = RoleEnum.Manager },
                new UserRole { UserId = 3, RoleId = RoleEnum.Employee },
                new UserRole { UserId = 4, RoleId = RoleEnum.Employee },
                new UserRole { UserId = 4, RoleId = RoleEnum.Manager },
                new UserRole { UserId = 5, RoleId = RoleEnum.Employee },
                new UserRole { UserId = 6, RoleId = RoleEnum.Manager },
            };

            // Mocking database operation
            _stubUserRepository = new Mock<IUserRepository>();
            _stubUserRepository.Setup(iUserRepository => iUserRepository.IsFieldInUseAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string columnName, string columnValue) =>
                {
                    if (columnName == "Email")
                    {
                        // Returns true if email exists (Can login)
                        return _userRepository.Any(u => u.Email == columnValue);
                    }
                    else if (columnName == "NationalIdentityCard" || columnName == "MobileNumber")
                    {
                        // Returns true if any of the below exists (Cannot register then)
                        return _userRepository.Any(u => u.Email.Equals(columnValue)
                        || u.NationalIdentityCard.Equals(columnValue)
                        || u.MobileNumber.Equals(columnValue));
                    }
                    return false;
                });

            _stubUserRepository.Setup(iUserRepository => iUserRepository.GetUserHashedPasswordAndSaltAsync(It.IsAny<string>()))
                .ReturnsAsync((string email) =>
                {
                    var user = _userRepository.FirstOrDefault(u => u.Email == email);
                    if (user != null)
                    {
                        return new PasswordDTO
                        {
                            HashedPassword = user.Password,
                            Salt = user.Salt
                        };
                    }
                    return new PasswordDTO();
                });

            _stubUserRepository.Setup(iUserRepository => iUserRepository.RegisterUserAsync(It.IsAny<User>()))
                .ReturnsAsync((User user) =>
                {
                    user.Id = (short)(_userRepository.Max(u => u.Id) + 1);
                    _userRepository.Add(user);
                    return true;
                });

            _stubUserRepository.Setup(iUserRepository => iUserRepository.GetAllDepartmentsAsync())
                .ReturnsAsync(_departmentRepository);

            _stubUserRepository.Setup(iUserRepository => iUserRepository.GetUserRolesAsync(It.IsAny<string>()))
                .ReturnsAsync((string email) =>
                {
                    var user = _userRepository.FirstOrDefault(u => u.Email == email);
                    if (user != null)
                    {
                        var userRoles = _userRoleRepository
                        .Where(ur => ur.UserId == user.Id)
                        .Select(ur => new UserRoleDTO
                        {
                            RoleId = (byte)ur.RoleId,
                            RoleName = ur.RoleId.ToString(),
                        })
                        .ToList();
                        return userRoles;
                    }
                    return new List<UserRoleDTO>();
                });

            _stubUserRepository.Setup(iUserRepository => iUserRepository.GetUserDataAsync(It.IsAny<string>(), It.IsAny<byte>()))
                .ReturnsAsync((string email, byte roleId) =>
                {
                    var user = _userRepository.FirstOrDefault(u => u.Email == email);
                    if (user != null)
                    {
                        var userManager = _userRepository.FirstOrDefault(u => u.Id == user.ManagerId);
                        if (userManager != null)
                        {
                            return new UserDTO
                            {
                                Id = user.Id,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                DepartmentId = user.DepartmentId,
                                ManagerId = user.ManagerId,
                                ManagerName = $"{userManager.FirstName} {userManager.LastName}",
                                ManagerEmail = userManager.Email,
                                Email = user.Email,
                                RoleId = roleId
                            };
                        }
                        else
                        {
                            return new UserDTO
                            {
                                Id = user.Id,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                DepartmentId = user.DepartmentId,
                                ManagerId = null,
                                ManagerName = null,
                                ManagerEmail = null,
                                Email = user.Email,
                                RoleId = roleId
                            };
                        }
                    }
                    return null;
                });

            _stubUserRepository.Setup(iUserRepository => iUserRepository.GetAllManagersFromDepartmentAsync(It.IsAny<byte>()))
                .ReturnsAsync((byte departmentId) =>
                {
                    var managerDTOs = _userRepository
                    .Where(u => u.DepartmentId == departmentId)
                    .Join(_userRoleRepository,
                    u => u.Id,
                    userRole => userRole.UserId,
                    (u, userRole) => new
                    {
                        UserId = u.Id,
                        UserFirstName = u.FirstName,
                        UserLastName = u.LastName,
                        UserRoleId = userRole.RoleId
                    })
                    .Where(joinResult => joinResult.UserRoleId == RoleEnum.Manager)
                    .Select(joinResult => new ManagerDTO
                    {
                        Id = joinResult.UserId,
                        FirstName = joinResult.UserFirstName,
                        LastName = joinResult.UserLastName
                    });
                    return managerDTOs;
                });

            _userService = new UserService(_stubUserRepository.Object);
        }

        [TestCase("john.doe@gmail.com", "password", true, "Login Successful")]
        [TestCase("john.doe@gmail.com", "wrongPassword", false, "Invalid Password")]
        [TestCase("unregistered.email@gmail.com", "password", false, "Email is not registered")]
        [TestCase("haniyyahmunsoor6@gmail.com", "haniyyah", true, "Login Successful")]
        public async Task AuthenticateLoginCredentialsAsync_ShouldReturnExpectedResult
            (string email, string password, bool expectedReesult, string expectedMessage)
        {
            // Act
            var result = await _userService.AuthenticateLoginCredentialsAsync(email, password);

            // Assert
            Assert.AreEqual(expectedReesult, result.Success);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestCase("Tom", "Cruise", "58323474", "T111111111111C", 3, 3, "tom.cruise@gmail.com", "1111", true, "Registration Successful")]
        [TestCase("Tom", "Cruise", "58328374", "T111111111111C", 3, 3, "tom.cruise@gmail.com", "1111", false, "Mobile Number is in use")]
        [TestCase("Tom", "Cruise", "51111111", "J456378976532D", 3, 3, "tom.cruise@gmail.com", "1111", false, "National Identity Card number is in use")]
        [TestCase("Tom", "Cruise", "51111111", "J111111111111D", 3, 3, "john.doe@gmail.com", "1111", false, "Email is in use")]
        public async Task RegisterUserAsync_ShouldReturnExpectedResult
            (string firstName, string lastName, string mobileNumber, string nationalIdentityCardNumber, byte departmentId, byte managerId,
            string email, string password, bool expectedSuccess, string expectedMessage)
        {
            // Arrange
            var newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                MobileNumber = mobileNumber,
                NationalIdentityCard = nationalIdentityCardNumber,
                DepartmentId = departmentId,
                ManagerId = managerId,
                Email = email
            };

            // Act
            var result = await _userService.RegisterUserAsync(newUser, password);

            // Assert
            Assert.AreEqual(expectedSuccess, result.Success);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [Test]
        public async Task GetAllDepartmentsAsync_ShouldReturnAllDepartmentsInTheSystem()
        {
            // Arrange
            var expectedDepartments = _departmentRepository;

            // Act
            var actualDepartments = await _userService.GetAllDepartmentsAsync();

            // Assert
            Assert.AreEqual(expectedDepartments, actualDepartments);
        }

        [Test]
        public async Task GetUserRolesAsync_ShouldReturnAListOfUserRoleDTO_WhenUserHasRoles()
        {
            // Arrange
            var email = "rushmee@gmail.com";
            var expectedResult = new List<UserRoleDTO>
            {
                new UserRoleDTO { RoleId = 1, RoleName = RoleEnum.Employee.ToString() },
                new UserRoleDTO { RoleId = 2, RoleName = RoleEnum.Manager.ToString() }
            };

            // Act
            var userRoles = (await _userService.GetUserRolesAsync(email)).ToList();

            // Assert
            Assert.AreEqual(expectedResult.Count, userRoles.Count);
            for (int i = 0; i < expectedResult.Count; i++)
            {
                Assert.AreEqual(expectedResult[i].RoleId, userRoles[i].RoleId);
                Assert.AreEqual(expectedResult[i].RoleName, userRoles[i].RoleName);
            }
        }

        [Test]
        public async Task GetUserRolesAsync_ShouldReturnAnEmptyListOfUserRoleDto_WhenUserHasNoRoles()
        {
            // Arrange
            var email = "unregistered.email@gmail.com";
            var expectedResult = new List<UserRoleDTO>();

            // Act
            var userRoles = (await _userService.GetUserRolesAsync(email)).ToList();

            // Assert
            Assert.AreEqual(expectedResult.Count, userRoles.Count);
            for (int i = 0; i < expectedResult.Count; i++)
            {
                Assert.AreEqual(expectedResult[i].RoleId, userRoles[i].RoleId);
                Assert.AreEqual(expectedResult[i].RoleName, userRoles[i].RoleName);
            }
        }

        [Test]
        public async Task GetUserDataAsync_ShouldReturnUserData_WhenUserHasManager()
        {
            // Arrange
            var email = "john.doe@gmail.com";
            var roleId = (byte)RoleEnum.Employee;
            var expectedUserData = new UserDTO
            {
                Id = 3,
                FirstName = "John",
                LastName = "Doe",
                DepartmentId = 1,
                ManagerId = 6,
                ManagerName = "Hello World",
                ManagerEmail = "hello.world@gmail.com",
                Email = "john.doe@gmail.com",
                RoleId = roleId
            };

            // Act
            var actualUserData = await _userService.GetUserDataAsync(email, roleId);

            // Assert
            Assert.NotNull(actualUserData);
            Assert.AreEqual(expectedUserData.Id, actualUserData.Id);
            Assert.AreEqual(expectedUserData.FirstName, actualUserData.FirstName);
            Assert.AreEqual(expectedUserData.LastName, actualUserData.LastName);
            Assert.AreEqual(expectedUserData.DepartmentId, actualUserData.DepartmentId);
            Assert.AreEqual(expectedUserData.ManagerId, actualUserData.ManagerId);
            Assert.AreEqual(expectedUserData.ManagerName, actualUserData.ManagerName);
            Assert.AreEqual(expectedUserData.ManagerEmail, actualUserData.ManagerEmail);
            Assert.AreEqual(expectedUserData.Email, actualUserData.Email);
            Assert.AreEqual(expectedUserData.RoleId, actualUserData.RoleId);
        }

        [Test]
        public async Task GetUserDataAsync_ShouldReturnUserData_WhenUserHasNoManager()
        {
            // Arrange
            var email = "junaid.edoo@gmail.com";
            var roleId = (byte)RoleEnum.Administrator;
            var expectedUserData = new UserDTO
            {
                Id = 1,
                FirstName = "Junaid",
                LastName = "Edoo",
                DepartmentId = null,
                ManagerId = null,
                ManagerName = null,
                ManagerEmail = null,
                Email = "junaid.edoo@gmail.com",
                RoleId = roleId
            };

            // Act
            var actualUserData = await _userService.GetUserDataAsync(email, roleId);

            // Assert
            Assert.NotNull(actualUserData);
            Assert.AreEqual(expectedUserData.Id, actualUserData.Id);
            Assert.AreEqual(expectedUserData.FirstName, actualUserData.FirstName);
            Assert.AreEqual(expectedUserData.LastName, actualUserData.LastName);
            Assert.AreEqual(expectedUserData.DepartmentId, actualUserData.DepartmentId);
            Assert.AreEqual(expectedUserData.ManagerId, actualUserData.ManagerId);
            Assert.AreEqual(expectedUserData.ManagerName, actualUserData.ManagerName);
            Assert.AreEqual(expectedUserData.ManagerEmail, actualUserData.ManagerEmail);
            Assert.AreEqual(expectedUserData.Email, actualUserData.Email);
            Assert.AreEqual(expectedUserData.RoleId, actualUserData.RoleId);
        }

        [Test]
        public async Task GetUserDataAsync_ShouldReturnNull_WhenUserNotFound()
        {
            // Arrange
            var email = "not.found@gmail.com";
            var roleId = (byte)RoleEnum.Employee;

            // Act
            var actualUserData = await _userService.GetUserDataAsync(email, roleId);

            // Assert
            Assert.IsNull(actualUserData);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task GetAllManagersFromDepartmentAsync_ShouldReturnManagerDTOInTheRequestedDepartment
            (byte departmentId)
        {
            // Arrange
            var expectedManagers = GetExpectedManagers(departmentId);
            // Act
            var managers = (await _userService.GetAllManagersFromDepartmentAsync(departmentId)).ToList();

            // Assert
            for (int i = 0; i < expectedManagers.Count; i++)
            {
                Assert.AreEqual(expectedManagers[i].Id, managers[i].Id);
                Assert.AreEqual(expectedManagers[i].FirstName, managers[i].FirstName);
                Assert.AreEqual(expectedManagers[i].LastName, managers[i].LastName);
            }
        }

        #region PrivateHelperMethod
        private User CreateUser(short id, string firstName, string lastName, string mobileNumber, string nationalIdentityCard,
        byte? departmentId, byte? managerId, string email, string password)
        {
            var salt = PasswordHashing.GenerateSalt();
            return new User
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                MobileNumber = mobileNumber,
                NationalIdentityCard = nationalIdentityCard,
                DepartmentId = departmentId,
                ManagerId = managerId,
                Email = email,
                Password = PasswordHashing.HashPassword(password, salt),
                Salt = salt
            };
        }


        private List<ManagerDTO> GetExpectedManagers(byte departmentId)
        {
            switch (departmentId)
            {
                case 1:
                    return new List<ManagerDTO>
                    {
                        new ManagerDTO { Id = 6, FirstName = "Hello", LastName = "World" }
                    };
                case 2:
                    return new List<ManagerDTO>
                    {
                        new ManagerDTO { Id = 2, FirstName = "Divesh", LastName = "Nugessur" },
                        new ManagerDTO { Id = 4, FirstName = "Rushmee", LastName = "Toolsee" }
                    };
                case 3:
                    return new List<ManagerDTO>();
            }
            return new List<ManagerDTO>();
        }
        #endregion
    }
}