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
    public class CompanyRepository: GenericRepository<Company>, ICompanyRepository
    {
        private ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;

        }



        public void Update(Company company)
        {
           _db.Companies.Update(company);
        }
    }
}
