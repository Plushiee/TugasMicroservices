using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderServices.DAL.Interfaces;
using OrderServices.Models;
using System.Data.SqlClient;
using Dapper;

namespace OrderServices.DAL
{
    public class CustomerDapper : ICustomer
    {
        private string GetConnectionString()
        {
            return "Data Source=.\\SQLEXPRESS; Initial Catalog=OrderDB;Integrated Security=true;TrustServerCertificate=false;";
        }

        public Customer Add(Customer obj)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"INSERT INTO Customers (CustomerName) 
                         VALUES (@CustomerName);
                         SELECT CAST(SCOPE_IDENTITY() as int)";
                var param = new { CustomerName = obj.CustomerName };
                try
                {
                    int newCustomerId = conn.ExecuteScalar<int>(query, param);

                    obj.CustomerId = newCustomerId;

                    return obj;
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

        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"DELETE FROM Customers WHERE CustomerId = @CustomerId";
                var param = new { CustomerId = id };
                try
                {
                    conn.Execute(query, param);
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

        public IEnumerable<Customer> GetAll()
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM Customers";
                try
                {
                    return conn.Query<Customer>(query);
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

        public Customer GetByCustomerId(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM Customers WHERE CustomerId = @CustomerId";
                var param = new { CustomerId = id };
                try
                {
                    var result = conn.QueryFirstOrDefault<Customer>(query, param);
                    if (result == null)
                    {
                        throw new ArgumentException("Customer not found");
                    }
                    else
                    {
                        return result;
                    }
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

        public IEnumerable<Customer> GetByCustomerName(string name)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM Customers WHERE CustomerName LIKE @CustomerName ORDER BY CustomerId";
                var param = new { CustomerName = '%' + name + '%' };
                try
                {
                    return conn.Query<Customer>(query, param);
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

        public void Update(Customer obj)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"UPDATE Customers SET CustomerName = @CustomerName WHERE CustomerId = @CustomerId";
                var param = new { CustomerName = obj.CustomerName, CustomerId = obj.CustomerId };
                try
                {
                    conn.Execute(query, param);
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