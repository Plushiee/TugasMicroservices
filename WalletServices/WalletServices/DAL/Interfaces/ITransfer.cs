using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletServices.Models;

namespace WalletServices.DAL.Interfaces
{
    public interface ITransfer : ICrudTransfer<Transfer>
    {
        IEnumerable<Transfer> GetByDate(string date);
        IEnumerable<Transfer> GetByBalance(float start, float end);
        IEnumerable<Transfer> GetByWalletIdTo(int id);
        IEnumerable<Transfer> GetByWalletIdFrom(int id);
    }
}