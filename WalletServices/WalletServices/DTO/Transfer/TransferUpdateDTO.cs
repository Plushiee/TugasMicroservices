using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletServices.DTO.Transfer
{
    public class TransferUpdateDTO
    {
        public int TransferId { get; set; }
        public int WalletIdTo { get; set; }
        public int WalletIdFrom { get; set; }
        public float Balance { get; set; }
        public string? Date { get; set; }
    }
}