﻿using Aiges.Core.Models;

namespace Aiges.MVC.Models
{
    public class ProjectDetailsViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Tags { get; set; }

        public string Description { get; set; }

        public string ProjectFile { get; set; }

        public DateTime LastUpdated { get; set; }

        public ProjectCategory Category { get; set; }
    }
}
