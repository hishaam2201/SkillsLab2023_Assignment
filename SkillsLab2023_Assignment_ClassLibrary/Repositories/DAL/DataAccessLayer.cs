using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Reflection;

namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.DAL
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

        /*public int ReturnNumOfRowsAffected(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection sqlConnection = CreateConnection())
            {
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        sqlCommand.Parameters.AddRange(parameters);
                    }

                    return sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public object ReturnFirstColumnOfFirstRow(string query, SqlParameter[] parameters)
        {
            using (SqlConnection sqlConnection = CreateConnection())
            {
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        sqlCommand.Parameters.AddRange(parameters);
                    }
                    return sqlCommand.ExecuteScalar();
                }
            }
        }

        public void ExecuteTransaction(out bool isSuccessful, SqlCommand command, SqlParameter[] parameters)
        {
            using (SqlConnection sqlConnection = CreateConnection())
            {
                using (SqlTransaction sqlTransaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        command.Connection = sqlConnection;
                        command.Transaction = sqlTransaction;

                        if (parameters != null && parameters.Length > 0)
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        command.ExecuteNonQuery();

                        sqlTransaction.Commit();
                        isSuccessful = true;
                    }
                    catch (Exception exception)
                    {
                        sqlTransaction.Rollback();
                        isSuccessful = false;
                        // TODO: Log error
                        throw;
                    }
                }
            }
        }

        public SqlParameter[] GetSqlParametersFromObject(object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            List<SqlParameter> parameters = new List<SqlParameter>();

            foreach (PropertyInfo property in properties)
            {
                parameters.Add(new SqlParameter($"@{property.Name}", property.GetValue(obj)));
            }
            return parameters.ToArray();
        }

        public SqlParameter[] GetSqlParametersFromObject(object obj, List<string> excludedProperties)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            List<SqlParameter> parameters = new List<SqlParameter>();

            foreach (PropertyInfo property in properties)
            {
                if (excludedProperties != null)
                {
                    if (!excludedProperties.Contains(property.Name))
                    {
                        parameters.Add(new SqlParameter($"@{property.Name}", property.GetValue(obj)));
                    }
                }
            }
            return parameters.ToArray();
        }

        public bool RecordExists(string query, SqlParameter[] parameters)
        {
            using (SqlConnection sqlConnection = CreateConnection())
            {
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddRange(parameters);

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
        }*/
    }
}
