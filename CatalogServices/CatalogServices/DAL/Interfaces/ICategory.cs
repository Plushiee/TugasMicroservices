using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogServices.Models;

namespace CatalogServices.DAL.Interfaces
{
    public interface ICategory : ICategoryCrud<Category>
    {
        IEnumerable<Category> GetByName(string name);
    }
}