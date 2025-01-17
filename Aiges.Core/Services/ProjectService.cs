using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiges.Core.Interfaces;
using Aiges.Core.Models;
using Aiges.Core.DTO;
using Aiges.Core.CustomExceptions;

namespace Aiges.Core.Services
{
    public class ProjectService
    {
        private readonly IProjectRepo _projectRepo;

        public ProjectService(IProjectRepo projectRepo)
        {
            _projectRepo = projectRepo;
        }

        public Project GetProjectById(int? projectId)
        {
            if (!projectId.HasValue)
            {
                throw new InvalidProjectException("Project ID cannot be null.", new ArgumentNullException(nameof(projectId)));
            }

            try
            {
                ProjectDto projectDto = _projectRepo.GetProjectDtoById(projectId.Value);
                return new Project(projectDto);
            }
            catch (InvalidProjectRepoException ex)
            {
                throw new InvalidProjectException(
                    "Failed to retrieve project by ID. There was an error in the repository.",
                    ex
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error in GetProjectById: {ex.Message}");
                throw;
            }
        }

        public List<Project> GetAllProjects()
        {
            try
            {
                List<ProjectDto> projects = _projectRepo.GetAllProjects();
                return Project.ConvertToProjects(projects);
            }
            catch (InvalidProjectRepoException ex)
            {
                throw new InvalidProjectException("Failed to fetch all projects. There was an error in the repository.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error in GetAllProjects: {ex.Message}");
                throw;
            }
        }

        public List<Project> GetAllProjectsFromUser(int? userId)
        {
            if (!userId.HasValue)
            {
                throw new InvalidUserException("User ID cannot be null.");
            }

            try
            {
                List<ProjectDto> projects = _projectRepo.GetAllProjectsFromUser(userId.Value);

                if (projects == null || projects.Count == 0)
                {
                    throw new InvalidUserException($"No projects found for the user ID {userId.Value}.");
                }

                return Project.ConvertToProjects(projects);
            }
            catch (InvalidProjectRepoException ex)
            {
                throw new InvalidUserException(
                    "Failed to retrieve projects for the user. There was an error in the repository.",
                    ex
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error in GetAllProjectsFromUser: {ex.Message}");
                throw;
            }
        }

        public List<Project> GetConceptProjects(int? userId)
        {
            if (!userId.HasValue)
            {
                throw new InvalidProjectException("User ID cannot be null.", new ArgumentNullException(nameof(userId)));
            }

            try
            {
                List<ProjectDto> projects = _projectRepo.GetConceptProjects(userId.Value);
                return Project.ConvertToProjects(projects);
            }
            catch (InvalidProjectRepoException ex)
            {
                throw new InvalidProjectException(
                    "Failed to retrieve concept projects. There was an error in the repository.",
                    ex
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error in GetConceptProjects: {ex.Message}");
                throw;
            }
        }

        public void AddUsersToProject(int projectId, List<int> userIds)
        {
            if (userIds == null || userIds.Count == 0)
            {
                throw new InvalidProjectException("User list cannot be null or empty.", new ArgumentNullException(nameof(userIds)));
            }

            try
            {
                _projectRepo.AddUsersToProject(projectId, userIds);
            }
            catch (InvalidProjectRepoException ex)
            {
                throw new InvalidProjectException(
                    "Failed to add users to the project. There was an error in the repository.",
                    ex
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error in AddUsersToProject: {ex.Message}");
                throw;
            }
        }

        public int AddProjectAsConcept(Project projectToAdd)
        {
            ValidateProject(projectToAdd);

            try
            {
                ProjectDto projectDto = new ProjectDto(projectToAdd);
                return _projectRepo.AddProjectAsConceptDto(projectDto);
            }
            catch (InvalidProjectRepoException ex)
            {
                throw new InvalidProjectException(
                    "Failed to add the project as a concept. There was an error in the repository.",
                    ex
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error in AddProjectAsConcept: {ex.Message}");
                throw;
            }
        }

        public void UpdateProject(Project projectToUpdate)
        {
            ValidateProject(projectToUpdate);

            try
            {
                ProjectDto projectDto = new ProjectDto(projectToUpdate);
                _projectRepo.UpdateProjectDto(projectDto);
            }
            catch (InvalidProjectRepoException ex)
            {
                throw new InvalidProjectException(
                    "Failed to update the project. There was an error in the repository.",
                    ex
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error in UpdateProject: {ex.Message}");
                throw;
            }
        }

        public void ValidateProject(Project project)
        {
            if (project == null)
            {
                throw new InvalidProjectException("The project cannot be null.", new ArgumentNullException(nameof(project)));
            }

            if (string.IsNullOrWhiteSpace(project.Title))
            {
                throw new InvalidProjectException("Project title cannot be empty.", null);
            }

            if (string.IsNullOrWhiteSpace(project.Description))
            {
                throw new InvalidProjectException("Project description cannot be empty.", null);
            }
        }

        public void AcceptProject(Project acceptedProject)
        {
            ValidateProject(acceptedProject);

            try
            {
                ProjectDto projectDto = new ProjectDto(acceptedProject);
                _projectRepo.AcceptProjectDto(projectDto);
            }
            catch (InvalidProjectRepoException ex)
            {
                throw new InvalidProjectException(
                    "Failed to accept the project. There was an error in the repository.",
                    ex
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error in AcceptProject: {ex.Message}");
                throw;
            }
        }
    }

}
