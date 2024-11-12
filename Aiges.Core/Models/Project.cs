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
        public int Id { get; private set; }

        public string Title { get; private set; }

        public string Tags { get; private set; }

        public string Description { get; private set; }

        public bool Concept { get; private set; }

        public string ProjectFile { get; private set; }

        public DateTime LastUpdated { get; private set; }

        public ProjectCategory Category { get; set; }

        public Project() { }

        public Project(int id, string title, string tags, string description, bool concept, string projectFile, DateTime lastUpdated, ProjectCategory category)
        {
            this.Id = id;
            this.Title = title;
            this.Tags = tags;
            this.Description = description;
            this.Concept = concept;
            this.ProjectFile = projectFile;
            this.LastUpdated = lastUpdated;
            this.Category = category;
        }

        public Project(ProjectDto projectDto)
        {
            Id = projectDto.Id;
            Title = projectDto.Title;
            Tags = projectDto.Tags;
            Description = projectDto.Description;
            Concept = projectDto.Concept;
            ProjectFile = projectDto.ProjectFile;
            LastUpdated = projectDto.LastUpdated;
            Category = projectDto.Category;
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
