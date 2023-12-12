using System;
using System.Data.SqlClient;


namespace Framework.DAL
{
    public interface IDataAccessLayer : IDisposable
    {
        SqlConnection CreateConnection();
    }
}
