using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.DataAccessLayer
{
    public class DataAccessLayer : IDataAccessLayer
    {
        private SqlConnection _connection;
        public SqlConnection CreateConnection()
        {
            try
            {
                var connectionString = ConfigurationManager.AppSettings["DbConnection"];
                if (!string.IsNullOrEmpty(connectionString))
                {
                    _connection = new SqlConnection(connectionString);
                    _connection.Open();
                    return _connection;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to find the connection string", ex);
            }
            throw new ApplicationException("Unable to find the connection string");
        }

        public void Dispose()
        {
            if (_connection != null && _connection.State.Equals(ConnectionState.Open))
            {
                _connection.Close();
            }
        }
    }
}
