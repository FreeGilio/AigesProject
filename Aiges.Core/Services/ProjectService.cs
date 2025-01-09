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
            try
            {
                if (!projectId.HasValue)
                {
                    throw new InvalidProjectException("Project ID cannot be null.", new ArgumentNullException(nameof(projectId)));
                }

                ProjectDto projectDto = _projectRepo.GetProjectDtoById(projectId.Value);
                return new Project(projectDto);
            }
            catch (InvalidProjectException ex)
            {
                Console.WriteLine($"Project Error: {ex.Message}");
                throw; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error in GetProjectById: {ex.Message}");
                throw; 
            }
        }


        public List<Project> GetAllProjects()
        {

            List<ProjectDto> projects = _projectRepo.GetAllProjects();
            return Project.ConvertToProjects(projects);
        }

        public List<Project> GetAllProjectsFromUser(int? userId)
        {
            try
            {
                if (!userId.HasValue)
                {
                    throw new InvalidUserException("User ID cannot be null.");
                }

                List<ProjectDto> projects = _projectRepo.GetAllProjectsFromUser(userId.Value);

                if (projects == null || projects.Count == 0)
                {
                    throw new InvalidUserException($"No projects found for the user ID {userId.Value}.");
                }

                return Project.ConvertToProjects(projects);
            }
            catch (InvalidUserException ex)
            {
                Console.WriteLine($"User Error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error in GetAllProjectsFromUser: {ex.Message}");
                throw;
            }
        }

        public List<Project> GetConceptProjects(int? userId)
        {
            try
            {
                if (!userId.HasValue)
                {
                    throw new InvalidProjectException("User ID cannot be null.", new ArgumentNullException(nameof(userId)));
                }

                List<ProjectDto> projects = _projectRepo.GetConceptProjects(userId.Value);
                return Project.ConvertToProjects(projects);
            }
            catch (InvalidProjectException ex)
            {
                Console.WriteLine($"Error Fetching Concept Projects: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error in GetConceptProjects: {ex.Message}");
                throw;
            }
        }

        public void AddUsersToProject(int projectId, List<int> userIds)
        {
            try
            {
                if (userIds == null || userIds.Count == 0)
                {
                    throw new InvalidProjectException("User list cannot be null or empty.", new ArgumentNullException(nameof(userIds)));
                }

                _projectRepo.AddUsersToProject(projectId, userIds);
            }
            catch (InvalidProjectException ex)
            {
                Console.WriteLine($"Error Adding Users to Project: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error in AddUsersToProject: {ex.Message}");
                throw;
            }
        }

        public int AddProjectAsConcept(Project projectToAdd)
        {
            try
            {
                ValidateProject(projectToAdd);

                ProjectDto projectDto = new ProjectDto(projectToAdd);
                return _projectRepo.AddProjectAsConceptDto(projectDto);
            }
            catch (InvalidProjectException ex)
            {
                Console.WriteLine($"Project Validation Error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error in AddProjectAsConcept: {ex.Message}");
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
            try
            {
                ValidateProject(acceptedProject);

                ProjectDto projectDto = new ProjectDto(acceptedProject);
                _projectRepo.AcceptProjectDto(projectDto);
            }
            catch (InvalidProjectException ex)
            {
                Console.WriteLine($"Project Acceptance Error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error in AcceptProject: {ex.Message}");
                throw;
            }
        }

    }
}
