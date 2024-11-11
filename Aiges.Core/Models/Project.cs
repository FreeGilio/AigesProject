using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiges.Core.DTO;

namespace Aiges.Core.Models
{
    public class Project
    {
        public int id { get; private set; }

        public string title { get; private set; }

        public string tags { get; private set; }

        public string description { get; private set; }

        public bool concept { get; private set; }

        public string projectFile { get; private set; }

        public DateTime lastUpdated { get; private set; }

        public ProjectCategory category { get; set; }

        public Project() { }

        public Project(int id, string title, string tags, string description, bool concept, string projectFile, DateTime lastUpdated, ProjectCategory category)
        {
            this.id = id;
            this.title = title;
            this.tags = tags;
            this.description = description;
            this.concept = concept;
            this.projectFile = projectFile;
            this.lastUpdated = lastUpdated;
            this.category = category;
        }

        public Project(ProjectDto projectDto)
        {
            id = projectDto.id;
            title = projectDto.title;
            tags = projectDto.tags;
            description = projectDto.description;
            concept = projectDto.concept;
            projectFile = projectDto.projectFile;
            lastUpdated = projectDto.lastUpdated;
            category = projectDto.category;
        }

        public static List<Project> ConvertToProjects(List<ProjectDto> projectDtos)
        {

            List<Project> projects = new List<Project>();

            try
            {
                foreach (ProjectDto projectDto in projectDtos)
                {
                    projects.Add(new Project(projectDto));
                }
            }
            catch (Exception ex)
            {

            }


            return projects;
        }
    }
}
