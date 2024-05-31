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
    public class CatProdDapper : ICatProd
    {
        private string GetConnectionString()
        {
            return "Data Source=.\\SQLEXPRESS; Initial Catalog=CatalogDB;Integrated Security=true;TrustServerCertificate=false;";
        }

        public IEnumerable<CatProd> GetAll()
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = "SELECT a.*, b.CategoryName FROM Products a INNER JOIN Categories b ON a.CategoryID = b.CategoryID";
                var outputs = conn.Query<CatProd>(strSql);
                return outputs;
            }
        }

        public CatProd GetByID(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = "SELECT a.*, b.CategoryName FROM Products a INNER JOIN Categories b ON a.CategoryID = b.CategoryID WHERE ProductID = @ProductID";
                var param = new { ProductID = id };
                var output = conn.QueryFirstOrDefault<CatProd>(strSql, param);
                if (output == null)
                {
                    throw new ArgumentException("Data Tidak Ditemukan");
                }
                return output;
            }
        }

        public IEnumerable<CatProd> GetByName(string name)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = "SELECT a.*, b.CategoryName FROM Products a INNER JOIN Categories b ON a.CategoryID = b.CategoryID WHERE a.Name LIKE @Name ORDER BY Name";
                var param = new { Name = "%" + name + "%" };
                var output = conn.Query<CatProd>(strSql, param);
                if (output == null)
                {
                    throw new ArgumentException("Data Tidak Ditemukan");
                }
                return output;
            }
        }

        public CatProd GetByCategoryID(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = "SELECT a.*, b.CategoryName FROM Products a INNER JOIN Categories b ON a.CategoryID = b.CategoryID WHERE a.CategoryID = @CategoryID ORDER BY Name";
                var param = new { CategoryID = id };
                var output = conn.QueryFirstOrDefault<CatProd>(strSql, param);
                if (output == null)
                {
                    throw new ArgumentException("Data Tidak Ditemukan");
                }
                return output;
            }
        }

        public IEnumerable<CatProd> GetByCategoryName(string catName)
        {
             using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = "SELECT a.*, b.CategoryName FROM Products a INNER JOIN Categories b ON a.CategoryID = b.CategoryID WHERE b.CategoryName LIKE @CategoryName ORDER BY CategoryName";
                var param = new { CategoryName = "%" + catName + "%" };
                var outputs = conn.Query<CatProd>(strSql, param);
                return outputs;
            }
        }

        public IEnumerable<CatProd> GetByDescription(string description)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = "SELECT a.*, b.CategoryName FROM Products a INNER JOIN Categories b ON a.CategoryID = b.CategoryID WHERE Description LIKE @Description ORDER BY Description";
                var param = new { Description = "%" + description + "%" };
                var outputs = conn.Query<CatProd>(strSql, param);
                return outputs;
            }
        }
    }
}