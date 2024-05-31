using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogServices.DTO.Category
{
    public class CategoryUpdateDTO
    {
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }
    }
}