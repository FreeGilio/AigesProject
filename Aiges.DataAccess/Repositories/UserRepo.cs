using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiges.Core.DTO;
using Aiges.Core.Interfaces;
using Aiges.Core.Models;
using Aiges.DataAccess.DB;

namespace Aiges.DataAccess.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly DatabaseConnection databaseConnection;
        public UserRepo(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public UserDto AttemptLogin(string? email, string? password)
        {
            UserDto result = null;

            databaseConnection.StartConnection(connection =>
            {
                string sql = "SELECT id, email, username, password, created_at, admin FROM [user] WHERE email = @Email AND password = @Password";
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.Add(new SqlParameter("@Email", email));
                    command.Parameters.Add(new SqlParameter("@Password", password));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = MapUserFromReader(reader);
                        }
                    }
                }
            });

            return result;
        }

        public List<UserDto> GetAllUsers()
        {
            List<UserDto> users = new List<UserDto>();

            databaseConnection.StartConnection(connection =>
            {
                string sql = "SELECT id, email, username, password FROM [user] ORDER BY username ASC";
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(MapUserFromReader(reader));
                    }
                }
            });

            return users;
        }

        public UserDto GetUserDtoById(int userId)
        {
            UserDto result = null;

            databaseConnection.StartConnection(connection =>
            {
                string sql = @"
               SELECT id, email, username, password, created_at, admin FROM [user] WHERE Id = @Id";
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.Add(new SqlParameter("@Id", userId));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = MapUserFromReader(reader);
                        }
                    }
                }
            });

            return result;
        }

        public UserDto GetCreatorUserDto(int projectId)
        {
            UserDto result = null;

            databaseConnection.StartConnection(connection =>
            {
                string sql = @"
                SELECT TOP 1 u.id AS Id, u.username AS Username, u.admin AS Admin
                FROM Collaborator c
                INNER JOIN [User] u ON c.user_id = u.id
                WHERE c.project_id = @ProjectId
                ORDER BY c.user_id";

                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.Add(new SqlParameter("@ProjectId", projectId));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = new UserDto
                            {
                                Id = (int)reader["id"],                             
                                Username = (string)reader["username"],
                                Admin = (bool)reader["admin"]
                            };
                        }
                    }
                }
            });

            return result;
        }


        private UserDto MapUserFromReader(SqlDataReader reader)
        {
            return new UserDto
            {
                Id = (int)reader["id"],
                Email = (string?)reader["email"],
                Username = (string)reader["username"],
                Password = (string)reader["password"],
                CreatedAt = (DateTime)reader["created_at"],
                Admin = (bool)reader["admin"]
            };
        }
    }
}
