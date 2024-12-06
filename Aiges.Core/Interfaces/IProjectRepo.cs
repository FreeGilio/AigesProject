using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiges.Core.DTO;


namespace Aiges.Core.Interfaces
{
    public interface IProjectRepo
    {
        ProjectDto GetProjectDtoById(int projectId);

        public int AddProjectAsConceptDto(ProjectDto projectToAdd);

        void AcceptProjectDto(ProjectDto acceptedProject);
        List<ProjectDto> GetAllProjects();

        List<ProjectDto> GetAllProjectsFromUser(int userId);

        List<ProjectDto> GetConceptProjects(int userId);

        void AddUsersToProject(int projectId, List<int> userIds);

    }
}
