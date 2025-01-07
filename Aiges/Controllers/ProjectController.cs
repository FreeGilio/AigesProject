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
        private readonly ImageService imageService;
        private readonly ReplyService replyService;
        public ProjectController(ProjectService projectService, ProjectCategoryService projectCategoryService, UserService userService, ImageService imageService, ReplyService replyService)
        {
            this.projectService = projectService;
            this.projectCategoryService = projectCategoryService;
            this.userService = userService;
            this.imageService = imageService;
            this.replyService = replyService;
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

            List<Image> projectImages = imageService.GetImagesByProjectId(id);

            List<Reply> replies = replyService.GetRepliesByProjectId(id);

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
                Creator = creator,
                UploadedImages = projectImages.Select(img => img.Link).ToList(),
                Comments = replies,
                AddComment = null
            };

            return View(projectDetailsViewModel);
        }

        [HttpGet]
        public IActionResult AddProject()
        {
            var categories = projectCategoryService.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            int? loggedInUserId = HttpContext.Session.GetInt32("uId");
            if (loggedInUserId != null)
            {
                var creator = userService.GetUserById(loggedInUserId.Value); 
                var projectDetailsViewModel = new ProjectDetailsViewModel
                {
                    Creator = creator 
                };
                return View(projectDetailsViewModel);
            }

            return RedirectToAction("Login", "User");
        }


        [HttpPost]
        public IActionResult AddProject(ProjectDetailsViewModel newProject, List<IFormFile> uploadedFiles)
        {
            int? loggedInUserId = HttpContext.Session.GetInt32("uId");

            ModelState.Remove("Creator.Password");
            ModelState.Remove("Creator.Email");

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
                    ProjectFile = newProject.ProjectFile,
                    LastUpdated = DateTime.UtcNow
                });

                projectService.AddUsersToProject(newProjectId, newProject.UserIds);

                if (uploadedFiles != null && uploadedFiles.Any())
                {
                    foreach (var file in uploadedFiles)
                    {
                        if (file.Length > 0)
                        {
                            string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", uniqueFileName);

                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            imageService.AddImage(new Image
                            {
                                ProjectId = newProjectId,
                                Link = $"/images/{uniqueFileName}"
                            });
                        }
                    }
                }

                return RedirectToAction("ProjectDetails", new { id = newProjectId }); 
            }

            if (loggedInUserId != null)
            {
                var categories = projectCategoryService.GetAllCategories();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                var creator = userService.GetUserById(loggedInUserId.Value);
                newProject = new ProjectDetailsViewModel
                {
                    Creator = creator
                };
                return View(newProject);
            }


            return RedirectToAction("Login", "User");
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
