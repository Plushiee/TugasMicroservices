using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderServices.DTO.Wallet
{
    public class WalletUpdateBalanceDTO
    {
        public int WalletId { get; set; }
        public decimal Balance { get; set; }
    }
}