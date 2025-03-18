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
    public class ProductImageRepository :GenericRepository<ProductImage>, IProductImageRepository
    {
        private ApplicationDbContext _db;
        public ProductImageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;

        }

       

        public void Update(ProductImage obj)
        {
            _db.ProductImages.Update(obj);
        }
    }
}

