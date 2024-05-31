using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderServices.DTO.OrderHeader
{
    public class OrderHeaderAddDTO
    {
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public int WalletId { get; set; }
        public string? Password { get; set; }
    }
}