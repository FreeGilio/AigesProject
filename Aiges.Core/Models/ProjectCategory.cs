using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiges.Core.DTO;

namespace Aiges.Core.Models
{
    public class ProjectCategory
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ProjectCategory() { }

        public ProjectCategory(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public ProjectCategory(ProjectCategoryDto categoryDto)
        {
            Id = categoryDto.Id;
            Name = categoryDto.Name;
        }

        public static List<ProjectCategory> MapToCategories(List<ProjectCategoryDto> categoryDtos)
        {

            List<ProjectCategory> categories = new List<ProjectCategory>();

            try
            {
                foreach (ProjectCategoryDto categoryDto in categoryDtos)
                {
                    categories.Add(new ProjectCategory(categoryDto));
                }
            }
            catch (Exception ex)
            {

            }


            return categories;
        }
    }
}
