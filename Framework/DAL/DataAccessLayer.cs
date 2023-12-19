using System;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Threading.Tasks;

namespace Framework.DAL
{
    public class DataAccessLayer : IDataAccessLayer
    {
        private SqlConnection _connection;
        private const string DB_CONNECTION_STRING = "DbConnection";
        public async Task<SqlConnection> CreateConnectionAsync()
        {
            try
            {
                var connectionString = ConfigurationManager.AppSettings[DB_CONNECTION_STRING];
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new ApplicationException("Connection string is null or empty");
                }

                _connection = new SqlConnection(connectionString);
                await _connection.OpenAsync();
                return _connection;
            }
            catch (SqlException sqlException)
            {
                throw new ApplicationException("Error while creating a SQL connection", sqlException);
            }
            catch (Exception exception)
            {
                throw new ApplicationException("Error while creating a connection", exception);
            }
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
