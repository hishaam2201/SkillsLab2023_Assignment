using System;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;


namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.DAL
{
    public class DataAccessLayer : IDataAccessLayer
    {
        private SqlConnection _connection;
        private const string DB_CONNECTION_STRING = "DbConnection";
        public SqlConnection CreateConnection()
        {
            try
            {
                var connectionString = ConfigurationManager.AppSettings[DB_CONNECTION_STRING];
                if (!string.IsNullOrEmpty(connectionString))
                {
                    _connection = new SqlConnection(connectionString);
                    _connection.Open();
                    return _connection;
                }
            }
            catch (Exception exception)
            {
                throw new ApplicationException("Unable to find the connection string", exception);
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
