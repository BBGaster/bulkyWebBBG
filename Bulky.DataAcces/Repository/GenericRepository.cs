using Bulky.DataAcces.Data;
using Bulky.DataAcces.Repository.IGenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAcces.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbset;
        public GenericRepository(ApplicationDbContext db)
        {
            _db = db;
            this.dbset = _db.Set<T>();
            _db.products.Include(u => u.Category);
        }
        public T? Create(T obj)
        {
            dbset.Add(obj);
            return obj;
        }

        public IEnumerable<T> GetAll( string? includeProperties = null)
        {
            IQueryable<T> query = dbset;
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) 
                {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
        }

        public T? GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbset;
            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
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
