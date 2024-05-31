using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderServices.Models;

namespace OrderServices.DAL.Interfaces
{
    public interface ICrudCustomer<T>
    {
        IEnumerable<T> GetAll();
        T GetByCustomerId(int id);
        Customer Add(T obj);
        void Update(T obj);
        void Delete(int id);
    }
}