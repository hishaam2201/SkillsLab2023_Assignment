using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.DataAccessLayer
{
    public interface IDataAccessLayer : IDisposable
    {
        SqlConnection CreateConnection();
    }
}
