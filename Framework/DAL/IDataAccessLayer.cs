using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Framework.DAL
{
    public interface IDataAccessLayer : IDisposable
    { 
        Task<SqlConnection> CreateConnectionAsync();
    }
}
