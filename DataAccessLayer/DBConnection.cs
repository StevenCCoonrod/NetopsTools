using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
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
