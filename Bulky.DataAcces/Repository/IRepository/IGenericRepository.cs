using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAcces.Repository.IGenericRepository
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T? GetFirstOrDefault(Expression<Func<T,bool>> filter);
        void Remove(T obj);
        void RemoveRange(IEnumerable<T> obj);
        T? Create(T obj);
    }
}
