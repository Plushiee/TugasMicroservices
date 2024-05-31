using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogServices.DAL.Interfaces;
using CatalogServices.Models;
using System.Data.SqlClient;
using Dapper;
using CatalogServices.DTO.Product;

namespace CatalogServices.DAL
{
    public class ProductDapper : IProduct
    {
        private string GetConnectionString()
        {
            return "Data Source=.\\SQLEXPRESS; Initial Catalog=CatalogDB;Integrated Security=true;TrustServerCertificate=false;";
        }
        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"DELETE FROM Products WHERE ProductID = @ProductID";
                var param = new { ProductID = id };
                try
                {
                    conn.Execute(strSql, param);
                }
                catch (SqlException sqlEx)
                {
                    throw new ArgumentException($"Error: {sqlEx.Message} - {sqlEx.Number}");
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Error: {ex.Message}");
                }
            }
        }

        public IEnumerable<Product> GetAll()
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = "SELECT * FROM Products";
                var products = conn.Query<Product>(strSql);
                return products;
            }
        }

        public Product GetByID(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = "SELECT * FROM Products WHERE ProductID = @ProductID";
                var param = new { ProductID = id };
                var product = conn.QueryFirstOrDefault<Product>(strSql, param);
                return product;
            }
        }

        public IEnumerable<Product> GetByName(string name)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = "SELECT * FROM Products WHERE Name LIKE @ProductName ORDER BY Name";
                var param = new { ProductName = "%" + name + "%" };
                var products = conn.Query<Product>(strSql, param);
                if (products == null)
                {
                    throw new ArgumentException("Data Tidak Ditemukan");
                }
                return products;
            }
        }

        public Product GetByCategoryID(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = "SELECT * FROM Products WHERE CategoryID = @CategoryID";
                var param = new { CategoryID = id };
                var product = conn.QueryFirstOrDefault<Product>(strSql, param);
                if (product == null)
                {
                    throw new ArgumentException("Data Tidak Ditemukan");
                }
                return product;
            }
        }

        public IEnumerable<Product> GetByDescription(string description)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = "SELECT * FROM Products WHERE Description LIKE @Description ORDER BY Description";
                var param = new { Description = "%" + description + "%" };
                var products = conn.Query<Product>(strSql, param);
                return products;
            }
        }

        public void Insert(Product obj)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"INSERT INTO Products (CategoryID, Name, Description, Price, Quantity) 
                                VALUES (@CategoryID, @Name, @Description, @Price, @Quantity)";
                var param = new { CategoryID = obj.CategoryID, Name = obj.Name, Description = obj.Description, Price = obj.Price, Quantity = obj.Quantity };
                try
                {
                    conn.Execute(strSql, param);
                }
                catch (SqlException sqlEx)
                {
                    throw new ArgumentException($"Error: {sqlEx.Message} - {sqlEx.Number}");
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Error: {ex.Message}");
                }
            }
        }

        public void Update(Product obj)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"UPDATE Products SET CategoryID = @CategoryID, Name = @Name, Description = @Description, Price = @Price, Quantity = @Quantity WHERE ProductId = @ProductId";
                var param = new { CategoryID = obj.CategoryID, Name = obj.Name, Description = obj.Description, Price = obj.Price, Quantity = obj.Quantity, ProductId = obj.ProductID };
                try
                {
                    conn.Execute(strSql, param);
                }
                catch (SqlException sqlEx)
                {
                    throw new ArgumentException($"Error: {sqlEx.Message} - {sqlEx.Number}");
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Error: {ex.Message}");
                }
            }
        }

        public async Task UpdateStockAfterOrder(ProductUpdateQuantityDTO productUpdateQuantityDTO)
        {
            var strSql = @"UPDATE Products SET Quantity = Quantity - @Quantity WHERE ProductID = @ProductID";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var param = new { ProductID = productUpdateQuantityDTO.ProductID, Quantity = productUpdateQuantityDTO.Quantity };
                try
                {
                    await conn.ExecuteAsync(strSql, param);
                }
                catch (SqlException sqlEx)
                {
                    throw new ArgumentException($"Error: {sqlEx.Message} - {sqlEx.Number}");
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Error: {ex.Message}");
                }
            }
        }
    }
}