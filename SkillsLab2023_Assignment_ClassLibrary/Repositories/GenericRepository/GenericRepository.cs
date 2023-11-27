using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly IDataAccessLayer _dataAccessLayer;

        protected readonly string _tableName;
        protected readonly string _pkColumn;
        protected readonly string _insertSqlTemplate;
        protected readonly string _updateSqlTemplate;

        public GenericRepository(IDataAccessLayer dataAccessLayer)
        {
            _dataAccessLayer = dataAccessLayer;

            var obj = BaseEntity.CreateEmptyInstance<T>();
            _tableName = obj.TableName;
            _pkColumn = obj.PrimaryKeyColumn;
            _insertSqlTemplate = obj.InsertSqlTemplate;
            _updateSqlTemplate = obj.UpdateSqlTemplate;
        }

        public bool Create(T obj, out string message)
        {
            using (SqlConnection sqlConnection = _dataAccessLayer.CreateConnection())
            {
                using (SqlCommand sqlCommand = new SqlCommand(_insertSqlTemplate, sqlConnection))
                {
                    // Map property values to sql parameters
                    var relevantProperties = GetRelevantProperties(obj);
                    foreach (var property in relevantProperties)
                    {
                        sqlCommand.Parameters.AddWithValue($"@{property.Name}", property.GetValue(obj) ?? DBNull.Value);
                    }

                    try
                    {
                        int rowsAffected = sqlCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            message = "Insert Successful";
                            return true;
                        }
                        else
                        {
                            message = "An error occurred";
                            return false;
                        }
                    }
                    catch (SqlException exception)
                    {
                        message = $"Insert Failed: {exception.Message}";
                        return false;
                    }
                }
            }
        }

        public bool Update(int id, T obj, out string message)
        {
            using (SqlConnection sqlConnection = _dataAccessLayer.CreateConnection())
            {
                using (SqlCommand sqlCommand = new SqlCommand(_updateSqlTemplate, sqlConnection))
                {
                    // Map property values to sql parameters
                    var relevantProperties = GetRelevantProperties(obj);
                    foreach (var property in relevantProperties)
                    {
                        sqlCommand.Parameters.AddWithValue($"@{property.Name}", property.GetValue(obj)
                            ?? DBNull.Value);
                    }
                    sqlCommand.Parameters.AddWithValue("@Id", id);

                    try
                    {
                        int rowsAffected = sqlCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            message = "Update Successful";
                            return true;
                        }
                        else
                        {
                            message = "No records found to be updated";
                            return false;
                        }
                    }
                    catch (SqlException exception)
                    {
                        message = $"Update Failed: {exception.Message}";
                        return false;
                    }
                }
            }
        }

        public bool Delete(int id, out string message)
        {
            using (SqlConnection sqlConnection = _dataAccessLayer.CreateConnection())
            {
                string deleteQuery = $@"Delete from {_tableName} 
                                where {_pkColumn} = @Id ;";

                using (SqlCommand sqlCommand = new SqlCommand(deleteQuery, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@Id", id);

                    try
                    {
                        int rowsAffected = sqlCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            message = "Delete Successful";
                            return true;
                        }
                        else
                        {
                            message = "No records found to delete";
                            return false;
                        }
                    }
                    catch (SqlException exception)
                    {
                        message = $"Delete Failed: {exception.Message}";
                        return false;
                    }

                }
            }
        }

        public IEnumerable<T> GetAll()
        {
            using (SqlConnection sqlConnection = _dataAccessLayer.CreateConnection())
            {
                string queryAll = $@"SELECT * FROM {_tableName}; ";

                using (SqlCommand sqlCommand = new SqlCommand(queryAll, sqlConnection))
                {
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        return ReadResultSet(typeof(T), reader);
                    }
                }
            }

        }

        public T GetById(int id)
        {
            using (SqlConnection sqlConnection = _dataAccessLayer.CreateConnection())
            {
                string query = $"Select * from {_tableName} where {_pkColumn} = @Id";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        return ReadSingleResult(typeof(T), reader);
                    }
                }
            }
        }

        private static PropertyInfo[] GetRelevantProperties(T obj)
        {
            var properties = obj.GetType().GetProperties();

            return properties.Where(p => p.Name != "Id" && p.Name != "TableName" && p.Name != "PrimaryKeyColumn" &&
            p.Name != "InsertSqlTemplate" && p.Name != "UpdateSqlTemplate")
            .ToArray();
        }

        private static T CreateInstance(Type type, IDataReader reader)
        {
            var item = Activator.CreateInstance<T>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var classProperty = type.GetProperty(columnName);
                if (classProperty != null)
                {
                    var value = reader.GetValue(i);
                    classProperty.SetValue(item, value);
                }
            }
            return item;
        }

        private static IEnumerable<T> ReadResultSet(Type type, IDataReader reader)
        {
            var resultList = new List<T>();
            while (reader.Read())
            {
                var item = CreateInstance(type, reader);
                resultList.Add(item);
            }
            return resultList;
        }

        private static T ReadSingleResult(Type type, IDataReader reader)
        {
            if (reader.Read())
            {
                return CreateInstance(type, reader);
            }
            else
            {
                return default;
            }
        }
    }
}

