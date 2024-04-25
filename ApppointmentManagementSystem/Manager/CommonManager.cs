using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Interface.Repository;

namespace AppointmentManagementSystem.Manager
{
    public class CommonManager<T> : ICommonManager<T> where T : class
    {
        private ICommonRepository<T> repository;
        public CommonManager(ICommonRepository<T> common)
        {
            repository = common;
        }


        public bool Add(T entity)
        {
            return repository.Add(entity);
        }

        public bool Add(ICollection<T> entity)
        {
            return repository.Add(entity);
        }

        public bool Delete(T entity)
        {
            return repository.Delete(entity);
        }

        public bool Delete(ICollection<T> entity)
        {
            return repository.Delete(entity);
        }

        public ICollection<T> Get(System.Linq.Expressions.Expression<Func<T, bool>> predicate, params System.Linq.Expressions.Expression<Func<T, object>>[] includes)
        {
            return repository.Get(predicate, includes);
        }

        public ICollection<T> GetAll(params System.Linq.Expressions.Expression<Func<T, object>>[] include)
        {
            return repository.GetAll(include);
        }

        public T GetFirstOrDefault(System.Linq.Expressions.Expression<Func<T, bool>> predicate, params System.Linq.Expressions.Expression<Func<T, object>>[] includes)
        {
            return repository.GetFirstOrDefault(predicate, includes);
        }

        public bool Update(T entity)
        {
            return repository.Update(entity);
        }

        public bool Update(ICollection<T> entity)
        {
            return repository.Update(entity);
        }
    }
}
