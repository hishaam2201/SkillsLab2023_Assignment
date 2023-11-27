using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Services.GenericService
{
    public class GenericService<T> : IGenericService<T> where T : BaseEntity
    {
        private readonly IGenericRepository<T> _repository;

        public GenericService(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public bool Create(T obj, out string message)
        {
            bool result = _repository.Create(obj, out message);
            return result;
        }

        public bool Delete(int id, out string message)
        {
            bool result = _repository.Delete(id, out message);
            return result;
        }

        public IEnumerable<T> GetAll()
        {
            return _repository.GetAll();
        }

        public T GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool Update(int id, T obj, out string message)
        {
            bool result = _repository.Update(id, obj, out message);
            return result;
        }
    }
}
