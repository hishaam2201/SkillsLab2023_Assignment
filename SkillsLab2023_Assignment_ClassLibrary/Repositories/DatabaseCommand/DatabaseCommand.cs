using SkillsLab2023_Assignment_ClassLibrary.Repositories.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;


namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.DatabaseCommand
{
    public class DatabaseCommand :IDatabaseCommand
    {
        private readonly IDataAccessLayer _dataAccessLayer;
        public DatabaseCommand(IDataAccessLayer dataAccessLayer)
        {
            _dataAccessLayer = dataAccessLayer;
        }

        public void ExecuteTransaction(out bool isSuccessful, SqlCommand command, SqlParameter[] parameters)
        {
            using (SqlConnection sqlConnection = _dataAccessLayer.CreateConnection())
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
            using (SqlConnection sqlConnection = _dataAccessLayer.CreateConnection())
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
        }

        public object ReturnFirstColumnOfFirstRow(string query, SqlParameter[] parameters)
        {
            using (SqlConnection sqlConnection = _dataAccessLayer.CreateConnection())
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

        public int ReturnNumOfRowsAffected(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection sqlConnection = _dataAccessLayer.CreateConnection())
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
    }
}
