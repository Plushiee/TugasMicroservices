using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderServices.Models;

namespace OrderServices.DAL.Interfaces
{
    public interface ICrudOrderDetail<T>
    {
        IEnumerable<T> GetAll();
        T GetByOrderDetailId(int id);
        T GetByOrderHeaderId(int id);
        T GetByProductId(int id);
        OrderDetail Add(T obj);
        void Update(T obj);
        void Delete(int id);
    }
}