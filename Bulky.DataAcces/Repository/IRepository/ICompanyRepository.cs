using Bulky.DataAcces.Repository.IGenericRepository;
using Bulky.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAcces.Repository.IRepository
{
    public interface ICompanyRepository : IGenericRepository<Company> 
    {
        void Update(Company company);

    }
}
