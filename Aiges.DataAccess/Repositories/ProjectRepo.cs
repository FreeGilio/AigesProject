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
using Aiges.Core.CustomExceptions;

namespace Aiges.DataAccess.Repositories
{
    public class ProjectRepo : IProjectRepo
    {
        private readonly DatabaseConnection databaseConnection;

        public ProjectRepo(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        // Todo, Make a new custom exception for Sql errors and throw it to a Service exception, which throws a new exception to the controller 
        // and have the modelstate send a message

        public ProjectDto GetProjectDtoById(int projectId)
        {
            try
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
            catch (SqlException ex)
            {
                throw new InvalidProjectRepoException("Error occurred while fetching a project by ID in GetProjectDtoById.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidProjectRepoException("An unexpected error occurred in GetProjectDtoById.", ex);
            }
        }

        public List<ProjectDto> GetAllProjects()
        {
            try
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
            catch (SqlException ex)
            {
                throw new InvalidProjectRepoException("Error occurred while fetching all projects in GetAllProjects.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidProjectRepoException("An unexpected error occurred in GetAllProjects.", ex);
            }
        }

        public List<ProjectDto> GetConceptProjects(int userId)
        {
            try
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
            catch (SqlException ex)
            {
                throw new InvalidProjectRepoException("Error occurred while fetching concept projects in GetConceptProjects.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidProjectRepoException("An unexpected error occurred in GetConceptProjects.", ex);
            }
        }

        public List<ProjectDto> GetAllProjectsFromUser(int userId)
        {
            try
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
            catch (SqlException ex)
            {
                throw new InvalidProjectRepoException("Error occurred while fetching all user projects in GetAllProjectsFromUser.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidProjectRepoException("An unexpected error occurred in GetAllProjectsFromUser.", ex);
            }
        }

        public int AddProjectAsConceptDto(ProjectDto projectToAdd)
        {
            try
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
                        insertCommand.Parameters.Add(new SqlParameter("@LastUpdated", projectToAdd.LastUpdated));

                        newProjectId = (int)insertCommand.ExecuteScalar();
                    }
                });

                return newProjectId;
            }
            catch (SqlException ex)
            {
                throw new InvalidProjectRepoException("Error occurred while adding a concept project in AddProjectAsConceptDto.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidProjectRepoException("An unexpected error occurred in AddProjectAsConceptDto.", ex);
            }
        }

        public void UpdateProjectDto(ProjectDto projectToUpdate)
        {
            try
            {
                databaseConnection.StartConnection(connection =>
                {
                    string updateSql = @"
                UPDATE project 
                SET 
                    title = @Title, 
                    category_id = @Category_Id, 
                    tags = @Tags, 
                    description = @Description,  
                    projectfile = @ProjectFile, 
                    last_updated = @LastUpdated
                WHERE id = @Id;";
                    using (SqlCommand updateCommand = new SqlCommand(updateSql, (SqlConnection)connection))
                    {
                        updateCommand.Parameters.Add(new SqlParameter("@Title", projectToUpdate.Title));
                        updateCommand.Parameters.Add(new SqlParameter("@Category_Id", projectToUpdate.Category.Id));
                        updateCommand.Parameters.Add(new SqlParameter("@Tags", projectToUpdate.Tags));
                        updateCommand.Parameters.Add(new SqlParameter("@Description", projectToUpdate.Description));
                        updateCommand.Parameters.Add(new SqlParameter("@ProjectFile", projectToUpdate.ProjectFile));
                        updateCommand.Parameters.Add(new SqlParameter("@LastUpdated", projectToUpdate.LastUpdated));
                        updateCommand.Parameters.Add(new SqlParameter("@Id", projectToUpdate.Id));

                        updateCommand.ExecuteNonQuery();
                    }
                });
            }
            catch (SqlException ex)
            {
                throw new InvalidProjectRepoException("Error occurred while updating a project in UpdateProjectDto.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidProjectRepoException("An unexpected error occurred in UpdateProjectDto.", ex);
            }
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
            try
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
            catch (SqlException ex)
            {
                throw new InvalidProjectRepoException("Error occurred while accepting a project in AcceptProjectDto.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidProjectRepoException("An unexpected error occurred in AcceptProjectDto.", ex);
            }
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
