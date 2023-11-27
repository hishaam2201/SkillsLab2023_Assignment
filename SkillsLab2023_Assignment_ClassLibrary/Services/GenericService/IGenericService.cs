using SkillsLab2023_Assignment_ClassLibrary.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Services.GenericService
{
    public interface IGenericService<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        bool Create(T obj, out string message);
        bool Update(int id, T obj, out string message);
        bool Delete(int id, out string message);
    }
}
