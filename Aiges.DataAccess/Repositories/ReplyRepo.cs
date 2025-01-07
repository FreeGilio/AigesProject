using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiges.Core.DTO;
using Aiges.Core.Interfaces;
using Aiges.DataAccess.DB;

namespace Aiges.DataAccess.Repositories
{
    public class ReplyRepo : IReplyRepo
    {
        private readonly DatabaseConnection databaseConnection;

        public ReplyRepo(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public List<ReplyDto> GetRepliesByProjectIdDto(int projectId)
        {
            List<ReplyDto> result = new List<ReplyDto>();

            databaseConnection.StartConnection(connection =>
            {
                string sql = @"
                    SELECT r.id, r.user_id, r.project_id, r.event_id, r.date, r.comment
                    FROM Reply r
                    WHERE r.project_id = @ProjectId";

                using SqlCommand command = new SqlCommand(sql, (SqlConnection)connection);
                command.Parameters.Add(new SqlParameter("@ProjectId", projectId));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ReplyDto reply = MapReplyDtoFromReader(reader);
                        result.Add(reply);
                    }
                }
            });

            return result;
        }

        public int AddCommentDto(ReplyDto replyToAdd)
        {
            int newReplyId = 0;

            databaseConnection.StartConnection(connection =>
            {
                string insertSql = @"
                          INSERT INTO reply (user_id, project_id, event_id, date, comment) 
                          OUTPUT INSERTED.id
                          VALUES (@UserId, @ProjectId, @EventId, @Date, @Comment);";
                using (SqlCommand insertCommand = new SqlCommand(insertSql, (SqlConnection)connection))
                {
                    insertCommand.Parameters.Add(new SqlParameter("@UserId", replyToAdd.UserId));
                    insertCommand.Parameters.Add(new SqlParameter("@ProjectId", replyToAdd.ProjectId ?? (object)DBNull.Value));
                    insertCommand.Parameters.Add(new SqlParameter("@EventId", replyToAdd.EventId ?? (object)DBNull.Value));
                    insertCommand.Parameters.Add(new SqlParameter("@Date", replyToAdd.Date));
                    insertCommand.Parameters.Add(new SqlParameter("@Comment", replyToAdd.Comment));

                    newReplyId = (int)insertCommand.ExecuteScalar();

                }
            });
            return newReplyId;
        }

        private ReplyDto MapReplyDtoFromReader(SqlDataReader reader)
        {
            return new ReplyDto
            {
                Id = (int)reader["Id"],
                UserId = reader.IsDBNull(reader.GetOrdinal("user_id")) ? (int?)null : (int)reader["user_id"],
                ProjectId = reader.IsDBNull(reader.GetOrdinal("project_id")) ? (int?)null : (int)reader["project_id"],
                EventId = reader.IsDBNull(reader.GetOrdinal("event_id")) ? (int?)null : (int)reader["event_id"],
                Date = reader["date"] != DBNull.Value ? (DateTime)reader["date"] : DateTime.MinValue,
                Comment = reader.IsDBNull(reader.GetOrdinal("comment")) ? null : (string)reader["comment"]
            };

        }

    }
}
