using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogServices.Models;
using CatalogServices.DTO.Product;

namespace CatalogServices.DAL.Interfaces
{
    public interface IProduct : IProductCrud<Product>
    {
        IEnumerable<Product> GetByName(string name);
        IEnumerable<Product> GetByDescription(string description);
        Task UpdateStockAfterOrder(ProductUpdateQuantityDTO productUpdateQuantityDTO);
    }
}