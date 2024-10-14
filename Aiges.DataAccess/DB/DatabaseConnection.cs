using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiges.DataAccess.DB
{
    public class DatabaseConnection
    {
        public readonly string connectionString;
        public DatabaseConnection(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void StartConnection(Action<DbConnection> action)
        {
            using (DbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                action.Invoke(connection);
                connection.Close();
            }
        }
    }
}
