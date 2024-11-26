using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiges.Core.Models;
using Aiges.Core.DTO;
using Aiges.Core.Interfaces;

namespace Aiges.Core.Services
{
    public class ProjectCategoryService
    {

        private readonly IProjectCategoryRepo _categoryRepo;
        public ProjectCategoryService(IProjectCategoryRepo categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public ProjectCategory GetCategoryById(int? categoryId)
        {
            ProjectCategoryDto categoryDto = _categoryRepo.GetCategoryDtoById(categoryId.Value);     
            return new ProjectCategory(categoryDto);
        }

        public List<ProjectCategory> GetAllCategories()
        {

                List<ProjectCategoryDto> categoryDtos = _categoryRepo.GetAllCategories();
                return ProjectCategory.MapToCategories(categoryDtos);
           
        }
    }
}
