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
        public int id { get; set; }

        public string title { get; set; }

        public string tags { get; set; }

        public string description { get; set; }

        public bool concept { get; set; }

        public string projectFile { get; set; }

        public DateTime lastUpdated { get; set; }

        public ProjectCategory category { get; set; }

        public ProjectDto() { }

        public ProjectDto(Project project)
        {
            id = project.id;
            title = project.title;
            tags = project.tags;
            description = project.description;
            concept = project.concept;
            projectFile = project.projectFile;
            lastUpdated = project.lastUpdated;
            category = project.category;
        }      
    }
}
