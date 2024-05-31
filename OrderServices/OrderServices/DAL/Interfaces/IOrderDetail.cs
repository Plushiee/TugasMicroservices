using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderServices.Models;

namespace OrderServices.DAL.Interfaces
{
    public interface IOrderDetail : ICrudOrderDetail<OrderDetail>
    {
        
    }
}