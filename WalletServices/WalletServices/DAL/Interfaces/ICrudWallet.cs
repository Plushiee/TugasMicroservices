using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletServices.Models;

namespace WalletServices.DAL.Interfaces
{
    public interface ICrudWallet<T>
    {
        IEnumerable<T> GetAll();
        T GetByWalletId(int id);
        T TopUpWallet(int walletId, float balance);
        T MinusWallet(int walletId, float balance);
        Wallet Add(T obj);
        void Update(T obj);
        void Delete(int id);
    }
}