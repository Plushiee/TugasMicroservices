using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderServices.Models;

namespace OrderServices.DAL.Interfaces
{
    public interface ICustomer : ICrudCustomer<Customer>
    {
        IEnumerable<Customer> GetByCustomerName(string name);
    }

}