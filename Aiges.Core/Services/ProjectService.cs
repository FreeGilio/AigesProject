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

            ProjectDto projectDto = _projectRepo.GetProjectDtoById(projectId.Value);

            return new Project(projectDto);
        }

        public List<Project> GetAllProjects()
        {

            List<ProjectDto> projects = _projectRepo.GetAllProjects();
            return Project.ConvertToProjects(projects);
        }

        public List<Project> GetAllProjectsFromUser(int? userId)
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



        public List<Project> GetConceptProjects(int? userId)
        {
            List<ProjectDto> projects = _projectRepo.GetConceptProjects(userId.Value);
            return Project.ConvertToProjects(projects);
        }

        public void AddUsersToProject(int projectId, List<int> userIds)
        {
            _projectRepo.AddUsersToProject(projectId, userIds);
        }

        public int AddProjectAsConcept(Project projectToAdd)
        {

            ValidateProject(projectToAdd);

            ProjectDto projectDto = new ProjectDto(projectToAdd);
            int newProjectId = _projectRepo.AddProjectAsConceptDto(projectDto); 
            return newProjectId;
        }

        private void ValidateProject(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project), "The project cannot be null.");

            if (string.IsNullOrWhiteSpace(project.Title))
                throw new ArgumentException("Project title cannot be empty.", nameof(project.Title));

            if (string.IsNullOrWhiteSpace(project.Description))
                throw new ArgumentException("Project description cannot be empty.", nameof(project.Description));
        }


        public void AcceptProject(Project acceptedProject)
        {
            ProjectDto projectDto = new ProjectDto(acceptedProject);
            _projectRepo.AcceptProjectDto(projectDto);
        }
    }
}
