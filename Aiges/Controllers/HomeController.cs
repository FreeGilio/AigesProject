using Aiges.Core.Services;
using Aiges.Core.Interfaces;
using Aiges.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Aiges.Core.Models;
using Aiges.MVC.Models;

namespace Aiges.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProjectService projectService;

        public HomeController(ILogger<HomeController> logger, ProjectService projectService)
        {
            _logger = logger;
            this.projectService = projectService;  
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
