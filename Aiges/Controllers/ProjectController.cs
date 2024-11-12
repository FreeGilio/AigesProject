using Microsoft.AspNetCore.Mvc;
using Aiges.Core.Services;
using Aiges.MVC.Models;

namespace Aiges.MVC.Controllers
{
    public class ProjectController : Controller
    {

        private readonly ProjectService projectService;
        public ProjectController(ProjectService projectService)
        {
            this.projectService = projectService;
        }

        public IActionResult Index()
        {
            var projects = projectService.GetAllProjects();

            var projectViewModel = projects.Select(project => new ProjectViewModel
            {
                Id = project.Id,
                Title = project.Title,
                Tags = project.Tags,
            }).ToList();

            return View(projectViewModel);
        }

        public ActionResult ProjectDetails(int id)
        {
            var project = projectService.GetProjectById(id);

            var projectDetailsViewModel = new ProjectDetailsViewModel
            {
                Id = project.Id,
                Title = project.Title,
                Tags = project.Tags,
                Description = project.Description,
                ProjectFile = project.ProjectFile,
                LastUpdated = project.LastUpdated,
                Category = project.Category,
            };

            return View(projectDetailsViewModel);
        }
    }
}
