using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
    /// <summary>
    /// CREATOR: Steve C
    /// Created: 2022/04/20
    /// Holds the SQL Server Database connection string and the method to make the DB connection
    /// </summary>
    public static class DBConnection
    {
        private static string connectionString = @"Data Source=localhost\MSSQLSERVER02;Initial Catalog=NetopsToolsDB;Integrated Security=True;TrustServerCertificate=True";
        public static SqlConnection GetConnection()
        {
            var conn = new SqlConnection(connectionString);
            return conn;
        }
    }
}
