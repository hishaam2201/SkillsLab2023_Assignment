using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Entity
{
    public abstract class BaseEntity
    {
        public abstract string TableName { get; }
        public abstract string PrimaryKeyColumn { get; }
        public abstract string InsertSqlTemplate { get; }
        public abstract string UpdateSqlTemplate { get; }

        private static readonly Dictionary<string, BaseEntity> _entities
                            = new Dictionary<string, BaseEntity>();

        public static BaseEntity CreateEmptyInstance<T>()
        {
            var key = typeof(T).FullName;

            if (!_entities.ContainsKey(key))
            {
                var obj = Activator.CreateInstance<T>() as BaseEntity;
                _entities.Add(key, obj);
            }

            return _entities[key];
        }
    }
}
