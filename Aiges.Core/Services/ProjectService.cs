using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiges.Core.Interfaces;
using Aiges.Core.Models;
using Aiges.Core.DTO;

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
            List<ProjectDto> projects = _projectRepo.GetAllProjectsFromUser(userId.Value);
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

            ProjectDto projectDto = new ProjectDto(projectToAdd);
            int newProjectId = _projectRepo.AddProjectAsConceptDto(projectDto); 
            return newProjectId;
        }
    }
}
