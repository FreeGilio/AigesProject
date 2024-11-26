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
        public int Id { get; set; }

        public string Title { get; set; }

        public string Tags { get; set; }

        public string Description { get;  set; }

        public bool Concept { get;  set; }

        public string ProjectFile { get; set; }

        public DateTime LastUpdated { get;  set; }

        public ProjectCategory Category { get; set; }

        public List<int> UserIds { get; set; } = new List<int>();

        public Project() { }

        public Project(int id, string title, string tags, string description, bool concept, string projectFile, DateTime lastUpdated, ProjectCategory category, List<int> userIds)
        {
            Id = id;
            Title = title;
            Tags = tags;
            Description = description;
            Concept = concept;
            ProjectFile = projectFile;
            LastUpdated = lastUpdated;
            Category = category;
            UserIds = userIds;           
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
            UserIds = projectDto.UserIds ?? new List<int>();
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
