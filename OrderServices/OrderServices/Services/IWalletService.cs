using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderServices.Models;
using OrderServices.DTO.Wallet;

namespace OrderServices.Services
{
    public interface IWalletService
    {
        Task<IEnumerable<Wallet>> GetAllWallet();
        Task<Wallet> GetByWalletId(int WalletId);
        Task WalletTopUp(WalletUpdateBalanceDTO walletUpdateBalanceDTO);
        Task MinusWallet(WalletUpdateBalanceDTO walletUpdateBalanceDTO);
    }
}