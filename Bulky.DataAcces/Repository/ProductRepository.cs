using Bulky.DataAcces.Data;
using Bulky.DataAcces.GenericRepository;
using Bulky.DataAcces.Repository.IRepository;
using Bulky.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAcces.Repository
{
    public class ProductRepository : GenericRepository<Product> , IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;

        }

        public void Update(Product product)
        {
            _db.products.Update(product);
        }
    }
}
