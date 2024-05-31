using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletServices.Models;

namespace WalletServices.DAL.Interfaces
{
    public interface ICrudTransfer<T>
    {
        IEnumerable<T> GetAll();
        T GetByTransferId(int id);
        Transfer Add(T obj);
        void Update(T obj);
        void Delete(int id);
    }
}