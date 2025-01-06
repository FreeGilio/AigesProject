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
using System.Data;

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
                LEFT JOIN
                    ProjectCategory pc ON p.category_id = pc.Id
                WHERE 
                    p.id = @Id";
                using SqlCommand command = new SqlCommand(sql, (SqlConnection)connection);
                command.Parameters.Add(new SqlParameter("@Id", projectId));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {                                            
                            result = MapProjectDtoFromReader(reader);                       
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
                LEFT JOIN
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

        public List<ProjectDto> GetConceptProjects(int userId)
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
            LEFT JOIN
                ProjectCategory pc ON p.category_id = pc.Id
            WHERE 
                p.concept = 1
                AND EXISTS (
                    SELECT 1
                    FROM [User] u
                    WHERE u.id = @UserId AND u.admin = 1
                );";

                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.Add(new SqlParameter("@UserId", userId));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            projects.Add(MapProjectDtoFromReader(reader));
                        }
                    }
                }
            });

            return projects;
        }

        public List<ProjectDto> GetAllProjectsFromUser(int userId)
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
                    pc.name as name
                FROM 
                    Project p
                LEFT JOIN
                    ProjectCategory pc ON p.category_id = pc.Id
                JOIN 
                    Collaborator cb ON p.id = cb.project_id
                WHERE 
                    cb.user_id = @UserId;";

                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.Add(new SqlParameter("@UserId", userId));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            projects.Add(MapProjectDtoFromReader(reader));
                        }
                    }
                }
            });

            return projects;
        }

        public int AddProjectAsConceptDto(ProjectDto projectToAdd)
        {
            int newProjectId = 0;

            databaseConnection.StartConnection(connection =>
            {
                string insertSql = @"
                          INSERT INTO project (title, category_id, tags, description, concept, projectfile, last_updated) 
                          OUTPUT INSERTED.id
                          VALUES (@Title, @Category_Id, @Tags, @Description, @Concept, @ProjectFile, @LastUpdated);";
                using (SqlCommand insertCommand = new SqlCommand(insertSql, (SqlConnection)connection))
                {
                    insertCommand.Parameters.Add(new SqlParameter("@Title", projectToAdd.Title));
                    insertCommand.Parameters.Add(new SqlParameter("@Category_Id", projectToAdd.Category.Id));
                    insertCommand.Parameters.Add(new SqlParameter("@Tags", projectToAdd.Tags));
                    insertCommand.Parameters.Add(new SqlParameter("@Description", projectToAdd.Description));
                    insertCommand.Parameters.Add(new SqlParameter("@Concept", true));
                    insertCommand.Parameters.Add(new SqlParameter("@ProjectFile", projectToAdd.ProjectFile));
                    insertCommand.Parameters.Add(new SqlParameter("@LastUpdated", DateTime.Now));

                    newProjectId = (int)insertCommand.ExecuteScalar();

                }
            });
            return newProjectId;
        }

        public void AddUsersToProject(int projectId, List<int> userIds)
        {
            databaseConnection.StartConnection(connection =>
            {
                string insertSql = @"
                    INSERT INTO Collaborator (project_id, user_id)
                    SELECT @ProjectId, Id
                    FROM @UserIds
                    WHERE Id NOT IN (
                        SELECT user_id 
                        FROM Collaborator 
                        WHERE project_id = @ProjectId
                    )";

                using (SqlCommand insertCommand = new SqlCommand(insertSql, (SqlConnection)connection))
                {
                    insertCommand.Parameters.Add(new SqlParameter("@ProjectId", projectId));

                    if (userIds.Any())
                    {
                        var userIdParameter = new SqlParameter("@UserIds", SqlDbType.Structured)
                        {
                            TypeName = "dbo.IntList",
                            Value = CreateDataTableFromIds(userIds)
                        };
                        insertCommand.Parameters.Add(userIdParameter);
                    }

                    insertCommand.ExecuteNonQuery();
                }
            });
        }

        public void AcceptProjectDto(ProjectDto acceptedProject)
        {
            databaseConnection.StartConnection(connection =>
            {
                string updateSql = "UPDATE project SET concept = @Concept WHERE id = @Id";

                using (SqlCommand updateCommand = new SqlCommand(updateSql, (SqlConnection)connection))
                {
                    updateCommand.Parameters.Add(new SqlParameter("@Id", acceptedProject.Id));
                    updateCommand.Parameters.Add(new SqlParameter("@Concept", acceptedProject.Concept));
                    updateCommand.ExecuteNonQuery();
                }
            });
        }

        private ProjectDto MapProjectDtoFromReader(SqlDataReader reader)
        {
            return new ProjectDto
            {
                Id = (int)reader["Id"],
                Title = (string)reader["title"],
                Tags = reader["tags"] != DBNull.Value ? (string)reader["tags"] : string.Empty,
                Description = (string)reader["description"],
                Concept = reader["concept"] != DBNull.Value && (bool)reader["concept"],
                ProjectFile = reader["projectfile"] != DBNull.Value ? (string)reader["projectfile"] : string.Empty,
                LastUpdated = reader["last_updated"] != DBNull.Value ? (DateTime)reader["last_updated"] : DateTime.MinValue,
                Category = new ProjectCategory
                {
                    Id = reader["CategoryId"] != DBNull.Value ? (int)reader["CategoryId"] : 0,
                    Name = reader["name"] != DBNull.Value ? (string)reader["name"] : string.Empty,
                }
            };

        }

        private DataTable CreateDataTableFromIds(List<int> ids)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));

            foreach (var id in ids)
            {
                table.Rows.Add(id);
            }

            return table;
        }
    }
}
