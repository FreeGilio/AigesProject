﻿using Aiges.Core.Models;
using Aiges.Core.Services;
using Aiges.MVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aiges.MVC.Controllers
{
    public class UserController : Controller
    {

        private readonly UserService userService;
        private readonly ProjectService projectService;
        public UserController(UserService userService, ProjectService projectService)
        {
            this.userService = userService;
            this.projectService = projectService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                User output = userService.Login(model.Email, model.Password);

                if (output != null)
                {
                    HttpContext.Session.SetInt32("uId", output.Id);
                    HttpContext.Session.SetString("uName", output.Username);
                    HttpContext.Session.SetInt32("uAdmin", output.Admin ? 1 : 0);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
                return View(model);
            }

        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Profile()
        {
            User user = userService.GetUserById(HttpContext.Session.GetInt32("uId")); 
            var userProjects = projectService.GetAllProjectsFromUser(user.Id);
            var conceptProjects = projectService.GetConceptProjects(user.Id);

            ProfileViewModel profileViewModel = new ProfileViewModel
            {
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Admin = user.Admin,
                UserProjects = userProjects.Select(project => new ProjectDetailsViewModel
                {
                    Id = project.Id,
                    Title = project.Title,
                    Description = project.Description,
                    Tags = project.Tags,
                    LastUpdated = project.LastUpdated,
                    Concept = project.Concept,
                    Category = new ProjectCategory
                    {
                        Id = project.Category.Id,
                        Name = project.Category.Name
                    }
                }).ToList(),

                ConceptProjects = conceptProjects.Select(project => new ProjectDetailsViewModel
                {
                    Id = project.Id,
                    Title = project.Title,
                    Description = project.Description,
                    Tags = project.Tags,
                    LastUpdated = project.LastUpdated,
                    Concept = project.Concept,
                    Category = new ProjectCategory
                    {
                        Id = project.Category.Id,
                        Name = project.Category.Name
                    }
                }).ToList()
            };

            return View(profileViewModel);
        }
    }
}
