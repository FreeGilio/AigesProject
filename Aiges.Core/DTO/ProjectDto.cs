using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiges.Core.Models;

namespace Aiges.Core.DTO
{
    public class ProjectDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Tags { get; set; }

        public string Description { get; set; }

        public bool Concept { get; set; }

        public string ProjectFile { get; set; }

        public DateTime LastUpdated { get; set; }

        public ProjectCategory Category { get; set; }

        public List<int> UserIds { get; set; } = new List<int>();

        public ProjectDto() { }

        public ProjectDto(Project project)
        {
            Id = project.Id;
            Title = project.Title;
            Tags = project.Tags;
            Description = project.Description;
            Concept = project.Concept;
            ProjectFile = project.ProjectFile;
            LastUpdated = project.LastUpdated;
            Category = project.Category;
            UserIds = project.UserIds;
        }      
    }
}
