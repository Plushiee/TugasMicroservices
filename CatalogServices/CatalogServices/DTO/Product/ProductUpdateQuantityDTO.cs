using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogServices.DTO.Product
{
    public class ProductUpdateQuantityDTO
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
    }
}