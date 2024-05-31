using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderServices.Models;

namespace OrderServices.DAL.Interfaces
{
    public interface ICrudOrderHeader<T>
    {
        IEnumerable<T> GetAll();
        T GetByOrderHeaderId(int id);
        OrderHeader Add(T obj);
        void Update(T obj);
        void Delete(int id);
    }
}