
using Framework.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Framework.DatabaseCommand.DatabaseCommand
{
    public class DatabaseCommand<T> : IDatabaseCommand<T>
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
                    catch (Exception)
                    {
                        sqlTransaction.Rollback();
                        isSuccessful = false;
                        throw;
                    }
                }
            }
        }

        public bool RecordExists(string query, SqlParameter[] parameters)
        {
            using (SqlConnection sqlConnection = _dataAccessLayer.CreateConnection())
            {
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    try
                    {
                        if (parameters != null && parameters.Any())
                        {
                            sqlCommand.Parameters.AddRange(parameters);
                        }

                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            return reader.HasRows;
                        }
                    }
                    catch (Exception) { throw; }

                }
            }
        }

        public object ReturnFirstColumnOfFirstRow(string query, SqlParameter[] parameters)
        {
            using (SqlConnection sqlConnection = _dataAccessLayer.CreateConnection())
            {
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    try
                    {
                        if (parameters != null && parameters.Length > 0)
                        {
                            sqlCommand.Parameters.AddRange(parameters);
                        }
                        return sqlCommand.ExecuteScalar();
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                }
            }
        }

        public int ReturnNumOfRowsAffected(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection sqlConnection = _dataAccessLayer.CreateConnection())
            {
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    try
                    {
                        if (parameters != null && parameters.Length > 0)
                        {
                            sqlCommand.Parameters.AddRange(parameters);
                        }

                        return sqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        public IEnumerable<TResult> ExecuteSelectQuery<TResult>(string query = null, SqlParameter[] parameters = null,
            Func<IDataReader, TResult> mapFunction = null)
        {
            List<TResult> result = new List<TResult>();

            using (SqlConnection sqlConnection = _dataAccessLayer.CreateConnection())
            {
                try
                {
                    using (SqlCommand sqlCommand = new SqlCommand(query ?? $@"SELECT * FROM [{typeof(T).Name}]", sqlConnection))
                    {
                        if (parameters != null && parameters.Any())
                        {
                            sqlCommand.Parameters.AddRange(parameters);
                        }
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    TResult item;
                                    if (mapFunction != null)
                                    {
                                        item = mapFunction(reader);
                                    }
                                    else
                                    {
                                        item = Activator.CreateInstance<TResult>();
                                        MapColumnsToProperties(item, reader);
                                    }
                                    result.Add(item);
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return result;
        }

        public T GetById(int id)
        {
            using (SqlConnection sqlConnection = _dataAccessLayer.CreateConnection())
            {
                string GET_BY_ID_QUERY = $@"SELECT * FROM [{typeof(T).Name}] WHERE Id = @Id";
                try
                {
                    using (SqlCommand sqlCommand = new SqlCommand(GET_BY_ID_QUERY, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@Id", id);
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                T item = Activator.CreateInstance<T>();
                                MapColumnsToProperties(item, reader);
                                return item;
                            }
                            return default; // return null
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        // PUBLIC HELPER METHODS
        public SqlParameter[] GetSqlParametersFromObject(object obj, List<string> excludedProperties = null)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            List<SqlParameter> parameters = new List<SqlParameter>();

            foreach (PropertyInfo property in properties)
            {
                if (excludedProperties == null || !excludedProperties.Contains(property.Name))
                {
                    parameters.Add(new SqlParameter($"@{property.Name}", property.GetValue(obj)));
                }
            }
            return parameters.ToArray();
        }

        // PRIAVTE HELPER METHODS
        private static void MapColumnsToProperties<TResult>(TResult item, IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string columnName = reader.GetName(i);
                PropertyInfo property = typeof(TResult).GetProperty(columnName);

                if (property != null && !reader.IsDBNull(i))
                {
                    var value = reader[i] == DBNull.Value ? null : reader[i];
                    property.SetValue(item, value);
                }
            }
        }

        private static string GetFields()
        {
            PropertyInfo[] props = typeof(T).GetProperties();
            var propsList = props.Where(prop => !Attribute.IsDefined(prop, typeof(Attribute))).ToList();
            StringBuilder columnName = new StringBuilder();

            foreach (PropertyInfo property in propsList)
            {
                columnName.Append(property.Name + ",");
            }
            columnName.Remove(columnName.Length - 1, 1); // Remove last comma
            return columnName.ToString();
        }
    }
}

