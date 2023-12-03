using System;
using System.Data.SqlClient;


namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.DAL
{
    public interface IDataAccessLayer : IDisposable
    {
        SqlConnection CreateConnection();
    }
}
