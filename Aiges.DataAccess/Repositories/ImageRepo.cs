using System;
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
    public class ImageRepo : IImageRepo
    {
        private readonly DatabaseConnection databaseConnection;

        public ImageRepo(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public List<ImageDto> GetImagesByProjectIdDto(int projectId)
        {
            List<ImageDto> result = new List<ImageDto>();

            databaseConnection.StartConnection(connection =>
            {
                string sql = @"
                SELECT id, project_id, link, event_id
                FROM Image
                WHERE project_id = @ProjectId";

                using SqlCommand command = new SqlCommand(sql, (SqlConnection)connection);
                command.Parameters.Add(new SqlParameter("@ProjectId", projectId));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read()) 
                    {
                        ImageDto image = MapImageDtoFromReader(reader);
                        result.Add(image);
                    }
                }
            });

            return result;
        }


        public void AddImageDto(ImageDto imageToAdd)
        {
            if (imageToAdd == null)
                throw new ArgumentNullException(nameof(imageToAdd), "Image to add cannot be null.");

            if (string.IsNullOrEmpty(imageToAdd.Link))
                throw new ArgumentException("Image link cannot be null or empty.", nameof(imageToAdd.Link));

            databaseConnection.StartConnection(connection =>
            {
                string insertSql = @"
            INSERT INTO Image (project_id, event_id, link)
            VALUES (@ProjectId, @EventId, @Link)";

                using (SqlCommand insertCommand = new SqlCommand(insertSql, (SqlConnection)connection))
                {
                    insertCommand.Parameters.Add(new SqlParameter("@ProjectId", imageToAdd.ProjectId));
                    insertCommand.Parameters.Add(new SqlParameter("@EventId",
                        (object)imageToAdd.EventId ?? DBNull.Value)); 
                    insertCommand.Parameters.Add(new SqlParameter("@Link", imageToAdd.Link));

                    try
                    {
                        insertCommand.ExecuteNonQuery(); 
                    }
                    catch (SqlException ex)
                    {
                        Console.Error.WriteLine($"Database error occurred: {ex.Message}");
                        throw; 
                    }
                }
            });
        }

        private ImageDto MapImageDtoFromReader(SqlDataReader reader)
        {
            return new ImageDto
            {
                Id = (int)reader["Id"],  
                ProjectId = reader.IsDBNull(reader.GetOrdinal("project_id")) ? (int?)null : (int)reader["project_id"],
                EventId = reader.IsDBNull(reader.GetOrdinal("event_id")) ? (int?)null : (int)reader["event_id"],
                Link = reader.IsDBNull(reader.GetOrdinal("link")) ? null : (string)reader["link"]
            };

        }

    }
}
