using Microsoft.AspNetCore.Mvc;
using Aiges.Core.Services;
using Aiges.MVC.Models;
using Aiges.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using Aiges.Core.CustomExceptions;

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
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Index: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        public ActionResult ProjectDetails(int id)
        {
            try
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
            catch (InvalidProjectException ex)
            {
                Console.WriteLine($"Project Error in Details: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProjectDetails: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public IActionResult AddProject()
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddProject GET: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult AddProject(ProjectDetailsViewModel newProject)
        {
            try
            {
                int? loggedInUserId = HttpContext.Session.GetInt32("uId");

                ModelState.Remove("Creator.Password");
                ModelState.Remove("Creator.Email");
                ModelState.Remove("AddComment");

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
                        ProjectFile = newProject.ProjectFile,
                        LastUpdated = DateTime.UtcNow
                    });

                    projectService.AddUsersToProject(newProjectId, newProject.UserIds);

                    if (newProject.Files != null && newProject.Files.Any())
                    {
                        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        foreach (var file in newProject.Files)
                        {
                            if (file.Length > 0)
                            {
                                string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                                string filePath = Path.Combine(directoryPath, uniqueFileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
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
            catch (InvalidProjectException ex)
            {
                Console.WriteLine($"Project Validation Error in AddProject POST: {ex.Message}");
                ModelState.AddModelError(string.Empty, ex.Message);
                var categories = projectCategoryService.GetAllCategories();
                ViewBag.Categories = new SelectList(categories, "Id", "Name"); 

                return View(newProject); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddProject POST: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public IActionResult EditProject(int id)
        {
            try
            {
                int? loggedInUserId = HttpContext.Session.GetInt32("uId");

                var creator = userService.GetUserById(loggedInUserId.Value);

                Project project = projectService.GetProjectById(id);

                if (project == null)
                {
                    return NotFound("Project not found.");
                }

                if (creator.Id != loggedInUserId)
                {
                    return Forbid(); 
                }             
              
                var categories = projectCategoryService.GetAllCategories();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");

                var projectViewModel = new ProjectDetailsViewModel
                {
                    Id = project.Id,
                    Title = project.Title,
                    Tags = project.Tags,
                    Description = project.Description,
                    ProjectFile = project.ProjectFile,
                    LastUpdated = project.LastUpdated,
                    Creator = creator,
                    Category = project.Category,
                    UploadedImages = imageService.GetImagesByProjectId(id).Select(img => img.Link).ToList()
                };

                return View(projectViewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Edit GET: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult EditProject(ProjectDetailsViewModel updatedProject)
        {
            try
            {

                ModelState.Remove("Creator.Password");
                ModelState.Remove("Creator.Email");
                ModelState.Remove("AddComment");

                if (!ModelState.IsValid)
                {
                    var categories = projectCategoryService.GetAllCategories();
                    ViewBag.Categories = new SelectList(categories, "Id", "Name");
                    return View(updatedProject);
                }

                projectService.UpdateProject(new Project
                {
                    Id = updatedProject.Id,
                    Title = updatedProject.Title,
                    Tags = updatedProject.Tags,
                    Description = updatedProject.Description,
                    ProjectFile = updatedProject.ProjectFile,
                    LastUpdated = DateTime.UtcNow,
                    Category = new ProjectCategory
                    {
                        Id = updatedProject.Category.Id,
                        Name = updatedProject.Category.Name
                    }
                });

                if (updatedProject.Files != null && updatedProject.Files.Any())
                {
                    string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    foreach (var file in updatedProject.Files)
                    {
                        if (file.Length > 0)
                        {
                            string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                            string filePath = Path.Combine(directoryPath, uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            imageService.AddImage(new Image
                            {
                                ProjectId = updatedProject.Id,
                                Link = $"/images/{uniqueFileName}"
                            });
                        }
                    }
                }

                return RedirectToAction("ProjectDetails", new { id = updatedProject.Id });
            }
            catch (InvalidProjectException ex)
            {
                Console.WriteLine($"Project Validation Error in EditProject POST: {ex.Message}");
                ModelState.AddModelError(string.Empty, ex.Message); 

                var categories = projectCategoryService.GetAllCategories();
                ViewBag.Categories = new SelectList(categories, "Id", "Name"); 
                return View(updatedProject); 
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error in Edit POST: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost]
        public IActionResult Accept(int id)
        {
            try
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
            catch (InvalidProjectException ex)
            {
                Console.WriteLine($"Project Error in Accept: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Accept: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
