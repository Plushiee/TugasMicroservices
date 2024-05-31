using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogServices.DAL.Interfaces
{
    public interface ICategoryCrud<T>
    {
        IEnumerable<T> GetAll();
        T GetByID(int id);
        void Insert(T obj);
        void Update(T obj);
        void Delete(int id);
    }
}