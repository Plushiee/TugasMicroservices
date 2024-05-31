using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogServices.Models;

namespace CatalogServices.DAL.Interfaces
{
    public interface ICatProd : ICatProdCrud<CatProd>
    {
        IEnumerable<CatProd> GetByName(string name);
        IEnumerable<CatProd> GetByCategoryName(string catName);
        IEnumerable<CatProd> GetByDescription(string description);
    }
}