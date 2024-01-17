using BusinessLayer.Services.TrainingService;
using DAL.DTO;
using DAL.Models;
using DAL.Repositories.TrainingRepository;
using Framework.Enums;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SkillsLabAssignment_TestProject
{
    [TestFixture]
    public class TrainingServiceTests
    {
        private Mock<ITrainingRepository> _stubTrainingRepository;

        private TrainingService _trainingService;

        private List<Training> _trainingRepository;
        private List<DepartmentDTO> _departmentRepository;
        private List<PreRequisite> _preRequisiteRepository;
        private List<TrainingPreRequisite> _trainingPreRequisiteRepository;
        private List<Application> _applicationRepository;

        [SetUp]
        public void SetUp()
        {
            // Data present in "database"
            _trainingRepository = new List<Training>
            {
                CreateTraining(1, "Training 1", "Description of Training 1", DateTime.Now.Date.AddDays(4d), DateTime.Now.Date.AddDays(1d), 5, 1, false),
                CreateTraining(2, "Training 2", "Description of Training 2", DateTime.Now.Date.AddDays(10d), DateTime.Now.Date.AddDays(4d), 10, 2, false),
                CreateTraining(3, "Training 3", "Description of Training 3", DateTime.Now.Date.AddDays(5d), DateTime.Now.Date.AddDays(-1d), 5, 3, true),
                CreateTraining(4, "Training 4", "Description of Training 4", DateTime.Now.Date.AddDays(2d), DateTime.Now.Date.AddDays(-2d), 5, 4, true),
                CreateTraining(5, "Training 5", "Description of Training 5", DateTime.Now.Date.AddDays(20d), DateTime.Now.Date.AddDays(10d), 5, 5, false),
                CreateTraining(6, "Training 6", "Description of Training 6", DateTime.Now.Date.AddDays(20d), DateTime.Now.Date.AddDays(10d), 5, 1, false),
            };
            _departmentRepository = new List<DepartmentDTO>
            {
                new DepartmentDTO{ Id = 1, DepartmentName = "Product and Technology"},
                new DepartmentDTO{ Id = 2, DepartmentName = "Customer Support"},
                new DepartmentDTO{ Id = 3, DepartmentName = "Finance"},
                new DepartmentDTO{ Id = 4, DepartmentName = "Payroll"},
                new DepartmentDTO{ Id = 5, DepartmentName = "Services"},
            };
            _preRequisiteRepository = new List<PreRequisite>
            {
                new PreRequisite{ Id = 1, Name = "PreRequisite 1", PreRequisiteDescription = "Description for PreRequisite 1"},
                new PreRequisite{ Id = 2, Name = "PreRequisite 2", PreRequisiteDescription = "Description for PreRequisite 2"},
                new PreRequisite{ Id = 3, Name = "PreRequisite 3", PreRequisiteDescription = "Description for PreRequisite 3"},
                new PreRequisite{ Id = 4, Name = "PreRequisite 4", PreRequisiteDescription = "Description for PreRequisite 4"},
                new PreRequisite{ Id = 5, Name = "PreRequisite 5", PreRequisiteDescription = "Description for PreRequisite 5"},
                new PreRequisite{ Id = 6, Name = "PreRequisite 6", PreRequisiteDescription = "Description for PreRequisite 6"},
                new PreRequisite{ Id = 7, Name = "PreRequisite 7", PreRequisiteDescription = "Description for PreRequisite 7"},
                new PreRequisite{ Id = 8, Name = "PreRequisite 8", PreRequisiteDescription = "Description for PreRequisite 8"},
                new PreRequisite{ Id = 9, Name = "PreRequisite 9", PreRequisiteDescription = "Description for PreRequisite 9"},
                new PreRequisite{ Id = 10, Name = "PreRequisite 10", PreRequisiteDescription = "Description for PreRequisite 10"}
            };
            _trainingPreRequisiteRepository = new List<TrainingPreRequisite>
            {
                new TrainingPreRequisite { TrainingId = 1, PreRequisiteId = 1 },
                new TrainingPreRequisite { TrainingId = 1, PreRequisiteId = 2 },
                new TrainingPreRequisite { TrainingId = 2, PreRequisiteId = 3 },
                new TrainingPreRequisite { TrainingId = 3, PreRequisiteId = 5 },
                new TrainingPreRequisite { TrainingId = 3, PreRequisiteId = 7 },
                new TrainingPreRequisite { TrainingId = 4, PreRequisiteId = 4 },
                new TrainingPreRequisite { TrainingId = 5, PreRequisiteId = 6 },
                new TrainingPreRequisite { TrainingId = 6, PreRequisiteId = 1 },
                new TrainingPreRequisite { TrainingId = 6, PreRequisiteId = 2 },
                new TrainingPreRequisite { TrainingId = 6, PreRequisiteId = 4 },
            };
            _applicationRepository = new List<Application>
            {
                CreateApplication(1, ApplicationStatusEnum.Pending.ToString(), DateTime.Now.Date, 3, 1, null, null),
                CreateApplication(2, ApplicationStatusEnum.Approved.ToString(), DateTime.Now.Date, 4, 2, null, null),
                CreateApplication(3, ApplicationStatusEnum.Declined.ToString(), DateTime.Now.Date, 2, 3, "Pre-Requisites do not match", null),
                CreateApplication(4, ApplicationStatusEnum.Declined.ToString(), DateTime.Now.Date, 4, 4, "Declined due to capacity constraint", null),
                CreateApplication(5, ApplicationStatusEnum.Selected.ToString(), DateTime.Now.Date, 7, 5, null, DateTime.Now.Date.AddDays(10d))
            };

            // Mocking up database calls
            _stubTrainingRepository = new Mock<ITrainingRepository>();

            _stubTrainingRepository.Setup(iTrainingRepository => iTrainingRepository.GetUnappliedTrainingsAsync(It.IsAny<byte>(), It.IsAny<short>()))
                .ReturnsAsync((byte departmentId, short userId) =>
                {
                    var trainings = _trainingRepository
                    .Where(training => training.IsDeadlineExpired == false &&
                    !_applicationRepository.Any(application => application.UserId == userId && application.TrainingId == training.Id))
                    .OrderBy(
                        training => training.DepartmentId == departmentId ? 0 : 1)
                    .Select(t => new TrainingDTO
                    {
                        TrainingId = t.Id,
                        TrainingName = t.TrainingName,
                        Description = t.Description,
                        TrainingCourseStartingDateTime = t.TrainingCourseStartingDateTime,
                        DeadlineOfApplication = t.DeadlineOfApplication,
                        Capacity = t.Capacity,
                        DepartmentName = GetDepartmentNameForTraining(t),
                        IsDeadlineExpired = t.IsDeadlineExpired
                    });

                    return trainings;
                });

            _stubTrainingRepository.Setup(iTrainingRepository => iTrainingRepository.GetAllTrainingsAsync())
                .ReturnsAsync(() =>
                {
                    var listOfAllTrainings = _trainingRepository
                    .Select(t => new TrainingDTO
                    {
                        TrainingId = t.Id,
                        TrainingName = t.TrainingName,
                        Description = t.Description,
                        TrainingCourseStartingDateTime = t.TrainingCourseStartingDateTime,
                        DeadlineOfApplication = t.DeadlineOfApplication,
                        Capacity = t.Capacity,
                        DepartmentName = GetDepartmentNameForTraining(t),
                        IsDeadlineExpired = t.IsDeadlineExpired,
                        PreRequisites = GetTrainingPreRequisites(t).ToList(),
                    });

                    return listOfAllTrainings;
                });

            _stubTrainingRepository.Setup(iTrainingRepository => iTrainingRepository.GetTrainingByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int trainingId) =>
                {
                    var training = _trainingRepository.Where(t => t.Id == trainingId)
                        .Select(selectedTraining => new TrainingDTO
                        {
                            TrainingId = selectedTraining.Id,
                            TrainingName = selectedTraining.TrainingName,
                            Description = selectedTraining.Description,
                            TrainingCourseStartingDateTime = selectedTraining.TrainingCourseStartingDateTime,
                            DeadlineOfApplication = selectedTraining.DeadlineOfApplication,
                            Capacity = selectedTraining.Capacity,
                            DepartmentName = GetDepartmentNameForTraining(selectedTraining),
                            IsDeadlineExpired = selectedTraining.IsDeadlineExpired,
                            PreRequisites = GetTrainingPreRequisites(selectedTraining).ToList()
                        }).FirstOrDefault();
                    return training;
                });

            _stubTrainingRepository.Setup(iTrainingRepository => iTrainingRepository.GetAllPreRequisitesAsync())
                .ReturnsAsync(_preRequisiteRepository);

            _stubTrainingRepository.Setup(iTrainingRepository => iTrainingRepository.HaveUsersAppliedForTrainingAsync(It.IsAny<int>()))
                .ReturnsAsync((int trainingId) =>
                {
                    return _applicationRepository.Any(app => app.TrainingId == trainingId);
                });

            _stubTrainingRepository.Setup(iTrainingRepository => iTrainingRepository.DeleteTrainingAsync(It.IsAny<int>()))
                .ReturnsAsync((int trainingId) =>
                {
                    var trainingItem = _trainingRepository.FirstOrDefault(t => t.Id == trainingId);
                    // Delete in training and in TrainingPreRequisites
                    bool arePreRequisitesDeleted = _trainingPreRequisiteRepository.RemoveAll(preReq => preReq.TrainingId == trainingId) > 0;
                    bool isTrainingDeleted = _trainingRepository.Remove(trainingItem);

                    if (arePreRequisitesDeleted && isTrainingDeleted)
                    {
                        return true;
                    }
                    return false;
                });

            _stubTrainingRepository.Setup(iTrainingRepository => iTrainingRepository.AddTrainingAsync(It.IsAny<Training>(), It.IsAny<string>()))
                .ReturnsAsync((Training training, string preRequisiteIds) =>
                {
                    _trainingRepository.Add(training);
                    string[] stringArray = preRequisiteIds.Split(',').Select(x => x.Trim()).ToArray();
                    int[] trainingPreRequisites = stringArray.Select(int.Parse).ToArray();

                    foreach (int i in trainingPreRequisites)
                    {
                        _trainingPreRequisiteRepository.Add(new TrainingPreRequisite { TrainingId = training.Id, PreRequisiteId = i });
                    }
                    return true;
                });

            _stubTrainingRepository.Setup(iTrainingRepository => iTrainingRepository.UpdateTrainingAsync(It.IsAny<Training>(), It.IsAny<string>()))
                .ReturnsAsync((Training training, string preRequisiteIds) =>
                {
                    var trainingToUpdate = _trainingRepository.FirstOrDefault(t => t.Id == training.Id);
                    if (trainingToUpdate != null)
                    {
                        trainingToUpdate.Id = training.Id;
                        trainingToUpdate.TrainingName = training.TrainingName;
                        trainingToUpdate.Description = training.Description;
                        trainingToUpdate.TrainingCourseStartingDateTime = training.TrainingCourseStartingDateTime;
                        trainingToUpdate.DeadlineOfApplication = training.DeadlineOfApplication;
                        trainingToUpdate.Capacity = training.Capacity;
                        trainingToUpdate.DepartmentId = training.DepartmentId;
                        trainingToUpdate.IsDeadlineExpired = training.IsDeadlineExpired;

                        bool arePreRequisitesDeleted = _trainingPreRequisiteRepository.RemoveAll(preReq => preReq.TrainingId == training.Id) > 0;
                        string[] stringPreReqIds = preRequisiteIds.Split(',').ToArray();
                        int[] intPreReqIds = stringPreReqIds.Select(int.Parse).ToArray();
                        foreach (int id in intPreReqIds)
                        {
                            _trainingPreRequisiteRepository.Add(new TrainingPreRequisite { TrainingId = training.Id, PreRequisiteId = id });
                        }
                        return true;
                    }
                    return false;
                });

            _trainingService = new TrainingService(_stubTrainingRepository.Object);
        }

        [Test]
        public async Task GetUnappliedTrainingsAsync_ShouldReturnEnumerableOfTrainingDTOs()
        {
            // Arrange
            byte userDepartmentId = 1;
            short userId = 2;
            var expectedTrainings = new List<TrainingDTO>
            {
                CreateTrainingDTO(1, "Training 1", "Description of Training 1", DateTime.Now.Date.AddDays(4d), DateTime.Now.Date.AddDays(1d), 5, "Product and Technology", false),
                CreateTrainingDTO(6, "Training 6", "Description of Training 6", DateTime.Now.Date.AddDays(20d), DateTime.Now.Date.AddDays(10d), 5, "Product and Technology", false),
                CreateTrainingDTO(2, "Training 2", "Description of Training 2", DateTime.Now.Date.AddDays(10d), DateTime.Now.Date.AddDays(4d), 10, "Customer Support", false),
                CreateTrainingDTO(5, "Training 5", "Description of Training 5", DateTime.Now.Date.AddDays(20d), DateTime.Now.Date.AddDays(10d), 5, "Services", false)
            };

            // Act
            var actualTrainings = (await _trainingService.GetUnappliedTrainingsAsync(userDepartmentId, userId)).ToList();

            // Assert
            for (int i = 0; i < expectedTrainings.Count; i++)
            {
                AssertTrainingDTO(expectedTrainings[i], actualTrainings[i]);
            }
        }

        [Test]
        public async Task GetAllTrainingsAsync_ShouldReturnAnEnumerableOfAllTrainingsAvailable()
        {
            // Arrange
            var preRequisitesForTraining1 = GetTrainingPreRequisites(new Training { Id = 1 });
            var preRequisitesForTraining2 = GetTrainingPreRequisites(new Training { Id = 2 });
            var preRequisitesForTraining3 = GetTrainingPreRequisites(new Training { Id = 3 });
            var preRequisitesForTraining4 = GetTrainingPreRequisites(new Training { Id = 4 });
            var preRequisitesForTraining5 = GetTrainingPreRequisites(new Training { Id = 5 });
            var expectedTrainings = new List<TrainingDTO>
            {
                CreateTrainingDTO(1, "Training 1", "Description of Training 1", DateTime.Now.Date.AddDays(4d), DateTime.Now.Date.AddDays(1d), 5, "Product and Technology", false, preRequisitesForTraining1.ToList()),
                CreateTrainingDTO(2, "Training 2", "Description of Training 2", DateTime.Now.Date.AddDays(10d), DateTime.Now.Date.AddDays(4d), 10, "Customer Support", false, preRequisitesForTraining2.ToList()),
                CreateTrainingDTO(3, "Training 3", "Description of Training 3", DateTime.Now.Date.AddDays(5d), DateTime.Now.Date.AddDays(-1d), 5, "Finance", true, preRequisitesForTraining3.ToList()),
                CreateTrainingDTO(4, "Training 4", "Description of Training 4", DateTime.Now.Date.AddDays(2d), DateTime.Now.Date.AddDays(-2d), 5, "Payroll", true, preRequisitesForTraining4.ToList()),
                CreateTrainingDTO(5, "Training 5", "Description of Training 5", DateTime.Now.Date.AddDays(20d), DateTime.Now.Date.AddDays(10d), 5, "Services", false, preRequisitesForTraining5.ToList()),
            };

            // Act
            var actualTrainings = (await _trainingService.GetAllTrainingsAsync()).ToList();

            // Assert
            for (int i = 0; i < expectedTrainings.Count; i++)
            {
                AssertTrainingDTO(expectedTrainings[i], actualTrainings[i]);
            }
        }

        [Test]
        public async Task GetTrainingByIdAsync_ShouldReturnDetailsOfTraining()
        {
            // Arrange
            short trainingId = 1;
            var preRequisitesForTraining1 = GetTrainingPreRequisites(new Training { Id = 1 });
            var expectedTraining = CreateTrainingDTO(trainingId, "Training 1", "Description of Training 1", DateTime.Now.Date.AddDays(4d), DateTime.Now.Date.AddDays(1d), 5, "Product and Technology", false, preRequisitesForTraining1.ToList());

            // Act
            var actualTraining = await _trainingService.GetTrainingByIdAsync(trainingId);

            // Assert
            AssertTrainingDTO(expectedTraining, actualTraining);
        }

        [Test]
        public async Task GetAllPreRequisitesAsync_ShouldReturnAllPreRequisitesInTheApplication()
        {
            // Arrange
            var expectedPreRequisites = _preRequisiteRepository;

            // Act
            var result = await _trainingService.GetAllPreRequisitesAsync();
            var actualPreRequisites = result.ListOfData as List<PreRequisite>;

            // Assert
            Assert.IsNotNull(actualPreRequisites);
            for (int i = 0; i < expectedPreRequisites.Count; i++)
            {
                Assert.AreEqual(expectedPreRequisites[i].Id, actualPreRequisites[i].Id);
                Assert.AreEqual(expectedPreRequisites[i].Name, actualPreRequisites[i].Name);
                Assert.AreEqual(expectedPreRequisites[i].PreRequisiteDescription, actualPreRequisites[i].PreRequisiteDescription);
            }
        }

        [Test]
        public async Task DeleteTrainingAsync_ShouldNotDeleteTrainingFromRepository_WhenUserHasAppliedForTraining()
        {
            // Arrange
            int trainingId = 1;
            int initialTrianingRepoCount = _trainingRepository.Count;
            int initialTrainingPreRequisiteCount = _trainingPreRequisiteRepository.Count;


            // Act
            var result = await _trainingService.DeleteTrainingAsync(trainingId);
            int afterDeleteTrainingRepoCount = _trainingRepository.Count;
            int afterDeleteTrainingPreRequisiteRepoCount = _trainingPreRequisiteRepository.Count;
            int trainingPreRequisiteCount = _trainingPreRequisiteRepository.Where(t => t.TrainingId == trainingId).Count();

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(initialTrianingRepoCount, afterDeleteTrainingRepoCount);
            Assert.AreEqual(initialTrainingPreRequisiteCount, afterDeleteTrainingPreRequisiteRepoCount);
            Assert.AreEqual("Employees have applied for this training.", result.Message);
        }

        [Test]
        public async Task DeleteTrainingAsync_ShouldDeleteTrainingFromRepository_WhenUserHasNotAppliedForTraining()
        {
            // Arrange
            int trainingId = 6;
            int initialTrianingRepoCount = _trainingRepository.Count;
            int initialTrainingPreRequisiteCount = _trainingPreRequisiteRepository.Count;
            int trainingPreRequisiteCount = _trainingPreRequisiteRepository.Where(t => t.TrainingId == trainingId).Count();

            // Act
            var result = await _trainingService.DeleteTrainingAsync(trainingId);
            int afterDeleteTrainingRepoCount = _trainingRepository.Count;
            int afterDeleteTrainingPreRequisiteCount = _trainingPreRequisiteRepository.Count;

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(initialTrianingRepoCount - 1, afterDeleteTrainingRepoCount);
            Assert.AreEqual(initialTrainingPreRequisiteCount - trainingPreRequisiteCount, afterDeleteTrainingPreRequisiteCount);
            Assert.AreEqual("Training deleted successfully.", result.Message);
        }

        [Test]
        public async Task SaveTrainingAsync_ShouldAddTraining_WhenIsUpdateIsFalse()
        {
            // Arrange
            var training = CreateTraining(7, "Training 7", "Description of training 7", DateTime.Now.Date.AddDays(10d), DateTime.Now.Date.AddDays(5d), 5, 2, false);
            var preRequisiteIds = "1, 2, 4";
            string[] preReqsIds = preRequisiteIds.Split(',').ToArray();
            var isUpdate = false;
            var initialTrainingRepoCount = _trainingRepository.Count;
            var initialTrainingPreRequisiteCount = _trainingPreRequisiteRepository.Count;


            // Act
            var result = await _trainingService.SaveTrainingAsync(training, preRequisiteIds, isUpdate);
            var afterAddTrainingRepoCount = _trainingRepository.Count;
            var afterAddTrainingPreRequisiteRepoCount = _trainingPreRequisiteRepository.Count;

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(initialTrainingRepoCount + 1, afterAddTrainingRepoCount);
            Assert.AreEqual(initialTrainingPreRequisiteCount + preReqsIds.Length, afterAddTrainingPreRequisiteRepoCount);
            Assert.AreEqual("Training added successfully.", result.Message);
        }

        [Test]
        public async Task SaveTrainingAsync_ShouldUpdateTraining_WhenIsUpdateIsTrue()
        {
            // Arrange
            var training = CreateTraining(6, "Training 6", "Description of training 6 with new description for testing", DateTime.Now.Date.AddDays(12d), DateTime.Now.Date.AddDays(-1d), 5, 2, true);
            var preRequisiteIds = "1, 2, 5";
            var isUpdate = true;
            var initialTrainingRepoCount = _trainingRepository.Count;
            var initialTrainingPreRequisiteCount = _trainingPreRequisiteRepository.Count;

            // Act
            var result = await _trainingService.SaveTrainingAsync(training, preRequisiteIds, isUpdate);
            var updatedTraining = _trainingRepository.FirstOrDefault(t => t.Id == training.Id);

            // Assert
            Assert.IsTrue(result.Success);
            AssertTraining(training, updatedTraining);
            Assert.AreEqual(initialTrainingRepoCount, _trainingRepository.Count);
            Assert.AreEqual(initialTrainingPreRequisiteCount, _trainingPreRequisiteRepository.Count);
            Assert.AreEqual("Training updated successfully.", result.Message);
        }

        [Test]
        public async Task SaveTrainingAsync_ShouldNotUpdateTraining_WhenIsUpdateInvalidIdIsUsed()
        {
            // Arrange
            var training = CreateTraining(7, "Training 7", "Description of training 7 with new description for testing", DateTime.Now.Date.AddDays(12d), DateTime.Now.Date.AddDays(-1d), 5, 2, true);
            var preRequisiteIds = "1, 2, 5";
            var isUpdate = true;

            // Act
            var result = await _trainingService.SaveTrainingAsync(training, preRequisiteIds, isUpdate);
            var updatedTraining = _trainingRepository.FirstOrDefault(t => t.Id == training.Id);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("An error occurred while updating training.", result.Message);
        }

        #region PRIVATE HELPER METHOD
        private Training CreateTraining(short id, string trainingName, string description, DateTime trainingStartDateTime, DateTime deadlineOfApplication,
            int capacity, byte departmentId, bool isDeadlineExpired)
        {
            return new Training
            {
                Id = id,
                TrainingName = trainingName,
                Description = description,
                TrainingCourseStartingDateTime = trainingStartDateTime,
                DeadlineOfApplication = deadlineOfApplication,
                Capacity = capacity,
                DepartmentId = departmentId,
                IsDeadlineExpired = isDeadlineExpired
            };
        }

        private TrainingDTO CreateTrainingDTO(short trainingId, string trainingName, string description, DateTime trainingCourseStartingDateTime
            , DateTime deadlineOfApplication, int capacity, string departmentName, bool isDeadlineExpired, List<TrainingPreRequisteDTO> preRequisites = null)
        {
            return new TrainingDTO
            {
                TrainingId = trainingId,
                TrainingName = trainingName,
                Description = description,
                TrainingCourseStartingDateTime = trainingCourseStartingDateTime,
                DeadlineOfApplication = deadlineOfApplication,
                Capacity = capacity,
                DepartmentName = departmentName,
                IsDeadlineExpired = isDeadlineExpired,
                PreRequisites = preRequisites
            };
        }


        private Application CreateApplication(int id, string applicationStatus, DateTime applicationDateTime, short userId,
            short trainingId, string declineReason, DateTime? selectedDate)
        {
            return new Application
            {
                Id = (short)id,
                ApplicationStatus = applicationStatus,
                ApplicationDateTime = applicationDateTime,
                UserId = userId,
                TrainingId = trainingId,
                DeclineReason = declineReason,
                SelectedDate = selectedDate
            };
        }

        private string GetDepartmentNameForTraining(Training t)
        {
            string departmentname = _departmentRepository
                .Where(d => d.Id == t.DepartmentId)
                .Select(d => d.DepartmentName)
                .FirstOrDefault();
            return departmentname;
        }

        private IEnumerable<TrainingPreRequisteDTO> GetTrainingPreRequisites(Training t)
        {
            var trainingPreRequisiteDTO = _trainingPreRequisiteRepository
                         .Where(tp => tp.TrainingId == t.Id)
                         .Join(_preRequisiteRepository,
                         trainingPreRequisite => trainingPreRequisite.PreRequisiteId,
                         preRequisite => preRequisite.Id,
                         (trainingPreRequisite, preRequisite) => new TrainingPreRequisteDTO
                         {
                             TrainingId = trainingPreRequisite.TrainingId,
                             PreRequisiteId = preRequisite.Id,
                             PreRequisiteName = preRequisite.Name,
                             PreRequisiteDescription = preRequisite.PreRequisiteDescription
                         });
            return trainingPreRequisiteDTO;
        }

        private void AssertTrainingDTO(TrainingDTO expected, TrainingDTO actual)
        {
            Assert.AreEqual(expected.TrainingId, actual.TrainingId);
            Assert.AreEqual(expected.TrainingName, actual.TrainingName);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.TrainingCourseStartingDateTime, actual.TrainingCourseStartingDateTime);
            Assert.AreEqual(expected.DeadlineOfApplication, actual.DeadlineOfApplication);
            Assert.AreEqual(expected.Capacity, actual.Capacity);
            Assert.AreEqual(expected.DepartmentName, actual.DepartmentName);
            Assert.AreEqual(expected.IsDeadlineExpired, actual.IsDeadlineExpired);

            if (expected.PreRequisites != null && expected.PreRequisites.Any())
            {
                for (int i = 0; i < expected.PreRequisites.Count; i++)
                {
                    Assert.AreEqual(expected.PreRequisites[i].PreRequisiteId, actual.PreRequisites[i].PreRequisiteId);
                    Assert.AreEqual(expected.PreRequisites[i].TrainingId, actual.PreRequisites[i].TrainingId);
                    Assert.AreEqual(expected.PreRequisites[i].PreRequisiteName, actual.PreRequisites[i].PreRequisiteName);
                    Assert.AreEqual(expected.PreRequisites[i].PreRequisiteDescription, actual.PreRequisites[i].PreRequisiteDescription);
                }
            }
        }

        private void AssertTraining(Training expected, Training actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.TrainingName, actual.TrainingName);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.TrainingCourseStartingDateTime, actual.TrainingCourseStartingDateTime);
            Assert.AreEqual(expected.DeadlineOfApplication, actual.DeadlineOfApplication);
            Assert.AreEqual(expected.Capacity, actual.Capacity);
            Assert.AreEqual(expected.DepartmentId, actual.DepartmentId);
            Assert.AreEqual(expected.IsDeadlineExpired, actual.IsDeadlineExpired);
        }
        #endregion
    }
}
