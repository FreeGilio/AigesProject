using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiges.Core.Interfaces;
using System.Data.SqlClient;
using Aiges.Core.DTO;
using Aiges.Core.Models;
using Aiges.DataAccess.DB;

namespace Aiges.DataAccess.Repositories
{
    public class ProjectRepo : IProjectRepo
    {
        private readonly DatabaseConnection databaseConnection;

        public ProjectRepo(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public ProjectDto GetProjectDtoById(int projectId)
        {
            ProjectDto result = null;

            databaseConnection.StartConnection(connection =>
            {
                string sql = @"
                SELECT
                    p.id as Id,
                    p.title,
                    p.tags,
                    p.description,
                    p.concept,
                    p.projectfile,
                    p.last_updated,
                    pc.id as CategoryId,
                    pc.name 
                FROM 
                    Project p
                JOIN
                    ProjectCategory pc ON p.category_id = pc.Id
                WHERE 
                    p.id = @Id";
                using SqlCommand command = new SqlCommand(sql, (SqlConnection)connection);
                command.Parameters.Add(new SqlParameter("@Id", projectId));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (string.IsNullOrEmpty(reader.GetString(1)))
                        {
                        }
                        else
                        {
                            result = MapProjectDtoFromReader(reader);
                        }
                    }
                }
            });

            return result;
        }

        public List<ProjectDto> GetAllProjects()
        {
            List<ProjectDto> projects = new List<ProjectDto>();

            databaseConnection.StartConnection(connection =>
            {
                string sql = @"
                SELECT
                    p.id as Id,
                    p.title,
                    p.tags,
                    p.description,
                    p.concept,
                    p.projectfile,
                    p.last_updated,
                    pc.id as CategoryId,
                    pc.name 
                FROM 
                    Project p
                JOIN
                    ProjectCategory pc ON p.category_id = pc.Id
                WHERE 
                    concept = 0;";
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        projects.Add(MapProjectDtoFromReader(reader));
                    }
                }
            });

            return projects;
        }

        private ProjectDto MapProjectDtoFromReader(SqlDataReader reader)
        {
            return new ProjectDto
            {
                Id = (int)reader["Id"],
                Title = (string)reader["title"],
                Tags = (string)reader["tags"],
                Description = (string)reader["description"],
                Concept = (bool)reader["concept"],
                ProjectFile = (string)reader["projectfile"],
                LastUpdated = (DateTime)reader["last_updated"],
                Category = new ProjectCategory
                {
                    Id = (int)reader["CategoryId"],
                    Name = (string)reader["name"],
                }
            };
        }
    }
}
