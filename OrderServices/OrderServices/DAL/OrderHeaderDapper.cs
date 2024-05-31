using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderServices.DAL.Interfaces;
using OrderServices.Models;
using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace OrderServices.DAL
{
    public class OrderHeaderDapper : IOrderHeader
    {
        private string GetConnectionString()
        {
            return "Data Source=.\\SQLEXPRESS; Initial Catalog=OrderDB;Integrated Security=true;TrustServerCertificate=false;";
        }

        public OrderHeader Add(OrderHeader obj)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"INSERT INTO OrderHeaders (CustomerId, OrderDate, WalletId) VALUES (@CustomerId, @OrderDate, @WalletId); SELECT CAST(SCOPE_IDENTITY() as int)";
                var param = new { CustomerId = obj.CustomerId, OrderDate = obj.OrderDate, WalletId = obj.WalletId};
                try
                {
                    int newOrderHeaderId = conn.ExecuteScalar<int>(query, param);

                    obj.OrderHeaderId = newOrderHeaderId;

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
                string query = @"DELETE FROM OrderHeaders WHERE OrderHeaderId = @OrderHeaderId";
                var param = new { OrderHeaderId = id };
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

        public IEnumerable<OrderHeader> GetAll()
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM OrderHeaders";
                try
                {
                    return conn.Query<OrderHeader>(query);
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

        public IEnumerable<OrderHeader> GetByCustomerId(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM OrderHeaders WHERE CustomerId = @CustomerId";
                var param = new { CustomerId = id };
                try
                {
                    var result = conn.Query<OrderHeader>(query, param);
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

        public IEnumerable<OrderHeader> GetByOrderDate(DateTime date)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM OrderHeaders WHERE OrderDate = @OrderDate";
                var param = new { OrderDate = date };
                try
                {
                    return conn.Query<OrderHeader>(query, param);
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

        public OrderHeader GetByOrderHeaderId(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM OrderHeaders WHERE OrderHeaderId = @OrderHeaderId";
                var param = new { OrderHeaderId = id };
                try
                {
                    var result = conn.QueryFirstOrDefault<OrderHeader>(query, param);
                    if (result == null)
                    {
                        throw new ArgumentException("Order not found");
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

        public void Update(OrderHeader obj)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"UPDATE OrderHeaders SET CustomerId = @CustomerId, OrderDate = @OrderDate, WalletId = @WalletId WHERE OrderHeaderId = @OrderHeaderId";
                var param = new { CustomerId = obj.CustomerId, OrderDate = obj.OrderDate, OrderHeaderId = obj.OrderHeaderId, WalletId = obj.WalletId };
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

        public IEnumerable<OrderHeader> GetByWalletId(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM OrderHeaders WHERE WalletId = @WalletId";
                var param = new { WalletId = id };
                try
                {
                    var result = conn.Query<OrderHeader>(query, param);
                    if (result == null)
                    {
                        throw new ArgumentException("Wallet not found");
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
    }
}