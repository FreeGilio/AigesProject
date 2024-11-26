using Microsoft.AspNetCore.Mvc;
using Aiges.Core.Services;
using Aiges.MVC.Models;
using Aiges.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Aiges.Core.DTO;

namespace Aiges.MVC.Controllers
{
    public class ProjectController : Controller
    {

        private readonly ProjectService projectService;
        private readonly ProjectCategoryService projectCategoryService;
        public ProjectController(ProjectService projectService, ProjectCategoryService projectCategoryService)
        {
            this.projectService = projectService;
            this.projectCategoryService = projectCategoryService;
        }

        public IActionResult Index()
        {
            List<Project> projects = projectService.GetAllProjects();

            List<ProjectViewModel> projectViewModel = projects.Select(project => new ProjectViewModel
            {
                Id = project.Id,
                Title = project.Title,
                Tags = project.Tags,
            }).ToList();

            return View(projectViewModel);
        }

        public ActionResult ProjectDetails(int id)
        {
            Project project = projectService.GetProjectById(id);

            ProjectDetailsViewModel projectDetailsViewModel = new ProjectDetailsViewModel
            {
                Id = project.Id,
                Title = project.Title,
                Tags = project.Tags,
                Description = project.Description,
                ProjectFile = project.ProjectFile,
                LastUpdated = project.LastUpdated,
                Concept = project.Concept,
                Category = project.Category
            };

            return View(projectDetailsViewModel);
        }

        [HttpGet]
        public IActionResult AddProject()
        {
            var categories = projectCategoryService.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(new ProjectDetailsViewModel());
        }

        [HttpPost]
        public IActionResult AddProject(ProjectDetailsViewModel newProject)
        {
            int? loggedInUserId = HttpContext.Session.GetInt32("uId");   

            if (ModelState.IsValid)
            {
                if (loggedInUserId != null && !newProject.UserIds.Contains(loggedInUserId.Value))
                {
                    newProject.UserIds.Add(loggedInUserId.Value);
                }

                int newProjectId = projectService.AddProjectAsConcept(new Project
                {
                    Title = newProject.Title,
                    Category = new ProjectCategory
                    {
                        Id = newProject.Category.Id,
                        Name = newProject.Category.Name
                    },
                    Tags = newProject.Tags,
                    Description = newProject.Description,
                    ProjectFile = newProject.ProjectFile
                });

                projectService.AddUsersToProject(newProjectId, newProject.UserIds);

                return RedirectToAction("ProjectDetails", new { id = newProjectId }); 
            }

            var categories = projectCategoryService.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(newProject); 
        }
    }
}
