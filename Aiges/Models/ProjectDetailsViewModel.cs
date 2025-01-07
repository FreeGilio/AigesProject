using Aiges.Core.DTO;
using Aiges.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Aiges.MVC.Models
{
    public class ProjectDetailsViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        public string Tags { get; set; }
        [Required]
        public string Description { get; set; }

        public string ProjectFile { get; set; }

        public DateTime LastUpdated { get; set; }

        public bool Concept {  get; set; }

        public ProjectCategory Category { get; set; }

        public List<int> UserIds { get; set; } = new List<int>();

        public User Creator { get; set; }

        public List<Reply> Comments { get; set; } = new List<Reply>();

        public AddCommentViewModel AddComment { get; set; }

        public List<string> UploadedImages { get; set; } = new List<string>();


        public bool HasTitle()
        {
            Project project = new Project
            {
                Title = this.Title,
            };
            return project.HasTitle();
        }

        public bool HasDescription()
        {
            Project project = new Project
            {
                Description = this.Description,
            };
            return project.HasDescription();
        }

    }
}
