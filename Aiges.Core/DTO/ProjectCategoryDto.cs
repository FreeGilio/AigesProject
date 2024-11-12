using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiges.Core.Models;

namespace Aiges.Core.DTO
{
    public class ProjectCategoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ProjectCategoryDto() { }

        public ProjectCategoryDto(ProjectCategory category)
        {
            Id = category.Id;
            Name = category.Name;
        }
    }
}
