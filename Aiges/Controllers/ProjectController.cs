using Microsoft.AspNetCore.Mvc;
using Aiges.Core.Models;
using Aiges.Core.Services;

namespace Aiges.MVC.Controllers
{
    public class ProjectController : Controller
    {

        private readonly ProjectService projectService;
        public ProjectController(ProjectService projectService)
        {
            this.projectService = projectService;
        }

        public ActionResult ProjectDetails(int id)
        {
            Project projectModel = projectService.GetProjectById(id);
            return View(projectModel);
        }
        public IActionResult Index()
        {
            var projects = projectService.GetAllProjects();
            return View(projects);
        }    
    }
}
