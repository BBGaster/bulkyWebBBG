using Bulky.DataAcces.Data;
using Bulky.DataAcces.Repository.IGenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAcces.GenericRepository
{
    public class GenericRepository<T> : Repository.IGenericRepository.IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbset;
        public GenericRepository(ApplicationDbContext db)
        {
            _db = db;
            this.dbset = _db.Set<T>();
        }
        public T? Create(T obj)
        {
            dbset.Add(obj);
            return obj;
        }

        public IEnumerable<T> GetAll()
        {
            IEnumerable<T> result = dbset.ToList();
            return result;
        }

        public T? GetFirstOrDefault(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbset;
            query = query.Where(filter);
            return query.FirstOrDefault();
        }

        public void Remove(T obj)
        {
            dbset.Remove(obj);
        }

        public void RemoveRange(IEnumerable<T> obj)
        {
            dbset.RemoveRange(obj);
        }
    }
}
