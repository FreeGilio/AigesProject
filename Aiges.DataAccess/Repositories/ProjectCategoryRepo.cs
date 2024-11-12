using Aiges.Core.Interfaces;
using Aiges.Core.DTO;
using Aiges.DataAccess.DB;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiges.DataAccess.Repositories
{
    public class ProjectCategoryRepo : IProjectCategoryRepo
    {
        private readonly DatabaseConnection databaseConnection;

        public ProjectCategoryRepo(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public ProjectCategoryDto GetCategoryDtoById(int categoryId)
        {
            ProjectCategoryDto result = null;

            databaseConnection.StartConnection(connection =>
            {
                string sql = "SELECT id, name FROM ProjectCategory WHERE Id = @Id;";
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.Add(new SqlParameter("@Id", categoryId));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.IsDBNull(0))
                            {
                            }
                            else
                            {
                                result = MapCategoryDtoFromReader(reader);
                            }
                        }
                    }
                }
            });

            return result;
        }

        public List<ProjectCategoryDto> GetAllCategories()
        {
            List<ProjectCategoryDto> categories = new List<ProjectCategoryDto>();

            databaseConnection.StartConnection(connection =>
            {
                string sql = "SELECT Id, Name FROM ProjectCategory";
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(MapCategoryDtoFromReader(reader));
                    }
                }
            });

            return categories;
        }

        private ProjectCategoryDto MapCategoryDtoFromReader(SqlDataReader reader)
        {
            return new ProjectCategoryDto
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"]
            };
        }
    }
}
