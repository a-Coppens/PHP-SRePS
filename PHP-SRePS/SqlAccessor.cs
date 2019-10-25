using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHP_SRePS
{
    static class SqlAccessor
    {
        static readonly SqlConnection connection;
        static readonly string dataSource = "php-sreps.database.windows.net";
        static readonly string userID = "swinAdmin";
        static readonly string password = "__admin12";
        static readonly string initialCatalog = "php-sreps";

        static SqlAccessor()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = dataSource,
                UserID = userID,
                Password = password,
                InitialCatalog = initialCatalog
            };
            connection = new SqlConnection(builder.ConnectionString);
        }

        public static void Open()
        {
            connection.Open();
        }

        public static void Close()
        {
            connection.Close();
        }

        /// <summary>
        /// Without Params. Must call between Open() and Close()
        /// </summary>
        public static SqlDataReader RunQuery(string query)
        {
            try
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    return reader;
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// With Params. Must call between Open() and Close()
        /// </summary>
        public static SqlDataReader RunQuery(string query, List<SqlParameter> parameters)
        {
            try
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());
                    SqlDataReader reader = command.ExecuteReader();
                    return reader;
                }
            } 
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
    }
}
