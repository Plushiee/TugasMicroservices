using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogServices.DAL.Interfaces
{
    public interface IProductCrud<T>
    {
        IEnumerable<T> GetAll();
        T GetByID(int id);
        T GetByCategoryID(int id);
        void Insert(T obj);
        void Update(T obj);
        void Delete(int id);
    }
}