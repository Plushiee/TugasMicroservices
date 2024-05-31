using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletServices.DTO.Transfer
{
    public class TransferAddDTO
    {
        public int WalletIdTo { get; set; }
        public int WalletIdFrom { get; set; }
        public string? Password { get; set; }
        public float Balance { get; set; }
    }
}