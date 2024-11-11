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
        public int id { get; set; }

        public string name { get; set; }

        public ProjectCategory() { }

        public ProjectCategory(int Id, string Name)
        {
            id = Id;
            name = Name;
        }

        public ProjectCategory(ProjectCategoryDto categoryDto)
        {
            id = categoryDto.id;
            name = categoryDto.name;
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
