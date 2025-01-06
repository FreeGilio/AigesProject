using Microsoft.AspNetCore.Mvc;
using Aiges.Core.Services;
using Aiges.MVC.Models;
using Aiges.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Aiges.Core.DTO;
using System.Reflection;

namespace Aiges.MVC.Controllers
{
    public class ProjectController : Controller
    {

        private readonly ProjectService projectService;
        private readonly ProjectCategoryService projectCategoryService;
        private readonly UserService userService;
        public ProjectController(ProjectService projectService, ProjectCategoryService projectCategoryService, UserService userService)
        {
            this.projectService = projectService;
            this.projectCategoryService = projectCategoryService;
            this.userService = userService;
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

            ViewData["IsAdmin"] = HttpContext.Session.GetInt32("uAdmin") == 1;

            Project project = projectService.GetProjectById(id);

            User creator = userService.GetCreatorUser(id);

            ProjectDetailsViewModel projectDetailsViewModel = new ProjectDetailsViewModel
            {
                Id = project.Id,
                Title = project.Title,
                Tags = project.Tags,
                Description = project.Description,
                ProjectFile = project.ProjectFile,
                LastUpdated = project.LastUpdated,
                Concept = project.Concept,
                Category = project.Category,
                Creator = creator
            };

            return View(projectDetailsViewModel);
        }

        [HttpGet]
        public IActionResult AddProject(Project model)
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

                if (!newProject.HasTitle() || !newProject.HasDescription())
                {
                    ModelState.AddModelError(string.Empty, "Both Title and Description must be filled in.");
                    return View(newProject);
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

        [HttpPost]
        public IActionResult Accept(int id)
        {
            Project project = projectService.GetProjectById(id);

            if (project == null)
            {
                return NotFound("Project not found.");
            }

            project.Concept = false; 

            projectService.AcceptProject(project);

            return RedirectToAction("ProjectDetails", new { id = project.Id });
        }

    }
}
