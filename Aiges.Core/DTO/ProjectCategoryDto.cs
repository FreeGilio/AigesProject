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
        public int id { get; set; }

        public string name { get; set; }

        public ProjectCategoryDto() { }

        public ProjectCategoryDto(ProjectCategory category)
        {
            id = category.id;
            name = category.name;
        }
    }
}
