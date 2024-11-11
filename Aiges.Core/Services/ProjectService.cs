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
            this._projectRepo = projectRepo;
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
    }
}
