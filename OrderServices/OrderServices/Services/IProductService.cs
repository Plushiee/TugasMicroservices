using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderServices.DTO.Product;
using OrderServices.Models;

namespace OrderServices.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProducts();
        Task<Product> GetByProductId(int ProductId);
        Task UpdateStockAfterOrder(ProductUpdateQuantityDTO productUpdateQuantityDTO);
    }
}