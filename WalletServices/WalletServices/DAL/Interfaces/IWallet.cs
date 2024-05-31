using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletServices.Models;

namespace WalletServices.DAL.Interfaces
{
    public interface IWallet : ICrudWallet<Wallet>
    {
        IEnumerable<Wallet> GetByUsername(string username);
        IEnumerable<Wallet> GetByEmail(string email);
        IEnumerable<Wallet> GetByFullName(string fullname);
        IEnumerable<Wallet> GetByBalance(float start, float end);
    }
}