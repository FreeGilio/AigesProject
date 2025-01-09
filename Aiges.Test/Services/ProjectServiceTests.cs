using Aiges.Core.CustomExceptions;
using Aiges.Core.DTO;
using Aiges.Core.Interfaces;
using Aiges.Core.Models;
using Aiges.Core.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiges.Test.Services
{
    public class ProjectServiceTests
    {
        [Fact]
        public void GetProjectById_ValidProjectId_ReturnsProject()
        {
            // Arrange
            var mockRepo = new Mock<IProjectRepo>();
            mockRepo.Setup(repo => repo.GetProjectDtoById(1))
                    .Returns(new ProjectDto { Id = 1, Title = "Sample Project" });
            var service = new ProjectService(mockRepo.Object);

            // Act
            var result = service.GetProjectById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Sample Project", result.Title);
        }

        [Fact]
        public void GetProjectById_InvalidId_ThrowsNullReferenceException()
        {
            // Arrange
            var mockRepo = new Mock<IProjectRepo>();
            mockRepo.Setup(repo => repo.GetProjectDtoById(It.IsAny<int>())).Returns((ProjectDto)null);
            var service = new ProjectService(mockRepo.Object);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => service.GetProjectById(99));
        }

        [Fact]
        public void AddUsersToProject_NullUserIds_ThrowsException()
        {
            // Arrange
            var mockRepo = new Mock<IProjectRepo>();
            var service = new ProjectService(mockRepo.Object);

            // Act & Assert
            Assert.Throws<InvalidProjectException>(() => service.AddUsersToProject(1, null));
        }

        [Fact]
        public void AddUsersToProject_EmptyUserIds_ThrowsException()
        {
            // Arrange
            var mockRepo = new Mock<IProjectRepo>();
            var service = new ProjectService(mockRepo.Object);

            // Act & Assert
            Assert.Throws<InvalidProjectException>(() => service.AddUsersToProject(1, new List<int>()));
        }



        [Fact]
        public void GetAllProjects_ReturnsProjectList()
        {
            // Arrange
            var mockRepo = new Mock<IProjectRepo>();
            mockRepo.Setup(repo => repo.GetAllProjects())
                    .Returns(new List<ProjectDto>
                    {
                new ProjectDto { Id = 1, Title = "Project 1" },
                new ProjectDto { Id = 2, Title = "Project 2" }
                    });
            var service = new ProjectService(mockRepo.Object);

            // Act
            var result = service.GetAllProjects();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetAllProjectsFromUser_NullUserId_ThrowsInvalidUserException()
        {
            // Arrange
            var mockRepo = new Mock<IProjectRepo>();
            var service = new ProjectService(mockRepo.Object);

            // Act & Assert
            Assert.Throws<InvalidUserException>(() => service.GetAllProjectsFromUser(null));
        }



        [Fact]
        public void GetConceptProjects_ReturnsConceptProjectList()
        {
            // Arrange
            var mockRepo = new Mock<IProjectRepo>();
            mockRepo.Setup(repo => repo.GetConceptProjects(1))
                    .Returns(new List<ProjectDto>
                    {
                new ProjectDto { Id = 1, Title = "Concept 1", Concept = true }
                    });
            var service = new ProjectService(mockRepo.Object);

            // Act
            var result = service.GetConceptProjects(1);

            // Assert
            Assert.Single(result);
            Assert.True(result[0].Concept);
        }

        [Fact]
        public void AddProjectAsConcept_EmptyTitle_ThrowsException()
        {
            // Arrange
            var service = new ProjectService(new Mock<IProjectRepo>().Object);
            var invalidProject = new Project { Title = "", Description = "Sample" };

            // Act & Assert
            Assert.Throws<InvalidProjectException>(() => service.AddProjectAsConcept(invalidProject));
        }

        [Fact]
        public void AddProjectAsConcept_NullDescription_ThrowsException()
        {
            // Arrange
            var service = new ProjectService(new Mock<IProjectRepo>().Object);
            var invalidProject = new Project { Title = "Valid Title", Description = null };

            // Act & Assert
            Assert.Throws<InvalidProjectException>(() => service.AddProjectAsConcept(invalidProject));
        }


        [Fact]
        public void ValidateProject_NullProject_ThrowsException()
        {
            // Arrange
            var service = new ProjectService(new Mock<IProjectRepo>().Object);

            // Act & Assert
            Assert.Throws<InvalidProjectException>(() => service.ValidateProject(null));
        }

        [Fact]
        public void AcceptProject_ValidProject_TransitionsConceptToFalse()
        {
            // Arrange
            var mockRepo = new Mock<IProjectRepo>();
            var service = new ProjectService(mockRepo.Object);

            var projectToAccept = new Project
            {
                Id = 1,
                Title = "Sample Project",
                Concept = false,
                Description = "Test Description"
            };

            // Act
            service.AcceptProject(projectToAccept);

            // Assert
            Assert.False(projectToAccept.Concept);
            mockRepo.Verify(repo => repo.AcceptProjectDto(It.IsAny<ProjectDto>()), Times.Once);
        }


    }
}
