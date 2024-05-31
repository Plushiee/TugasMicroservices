using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogServices.DAL.Interfaces;
using CatalogServices.Models;
using System.Data.SqlClient;
using Dapper;

namespace CatalogServices.DAL
{
    public class CategoryDapper : ICategory
    {
        private string GetConnectionString()
        {
            return "Data Source=.\\SQLEXPRESS; Initial Catalog=CatalogDB;Integrated Security=true;TrustServerCertificate=false;";
        }

        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"DELETE FROM Categories WHERE CategoryID = @CategoryID";
                var param = new { CategoryID = id };
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

        public IEnumerable<Category> GetAll()
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = "SELECT * FROM Categories ORDER BY CategoryID";
                var categories = conn.Query<Category>(strSql);
                return categories;
            }
        }

        public Category GetByID(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = "SELECT * FROM Categories WHERE CategoryID = @CategoryId";
                var param = new { CategoryId = id };
                var category = conn.QueryFirstOrDefault<Category>(strSql, param);
                if (category == null)
                {
                    throw new ArgumentException("Data Tidak Ditemukan");
                }
                return category;
            }
        }

        public IEnumerable<Category> GetByName(string name)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = "SELECT * FROM Categories WHERE CategoryName LIKE @CategoryName ORDER BY CategoryName";
                var param = new { CategoryName = $"%{name}%" };
                var categories = conn.Query<Category>(strSql, param);
                return categories;
            }
        }

        public void Insert(Category obj)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"INSERT INTO Categories (CategoryName) VALUES (@CategoryName)";
                var param = new { CategoryName = obj.CategoryName };
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

        public void Update(Category obj)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"UPDATE Categories SET CategoryName = @CategoryName WHERE CategoryID = @CategoryID";
                var param = new { CategoryName = obj.CategoryName, CategoryID = obj.CategoryID };
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
    }
}