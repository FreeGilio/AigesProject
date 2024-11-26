using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiges.Core.DTO;

namespace Aiges.Core.Interfaces
{
    public interface IProjectCategoryRepo
    {
        ProjectCategoryDto GetCategoryDtoById(int categoryId);

        List<ProjectCategoryDto> GetAllCategories();
    }
}
