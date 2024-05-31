using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletServices.Models;
using Dapper;
using WalletServices.DAL.Interfaces;
using System.Data.SqlClient;

namespace WalletServices.DAL
{
    public class TransferDapper : ITransfer
    {
        private string GetConnectionString()
        {
            return "Data Source=.\\SQLEXPRESS; Initial Catalog=WalletDB;Integrated Security=true;TrustServerCertificate=false;";
        }

        public Transfer Add(Transfer obj)
        {
            using (SqlConnection sqlConnection = new SqlConnection(GetConnectionString()))
            {
                string query = @"INSERT INTO Transfers (WalletIdFrom, WalletIdTo, Balance, Date) VALUES (@WalletIdFrom, @WalletIdTo, @Balance, @Date); SELECT CAST(SCOPE_IDENTITY() as int)";
                var param = new { WalletIdFrom = obj.WalletIdFrom, WalletIdTo = obj.WalletIdTo, Balance = obj.Balance, Date = obj.Date };
                try
                {
                    int newTransferId = sqlConnection.ExecuteScalar<int>(query, param);

                    obj.TransferId = newTransferId;

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
            using (SqlConnection sqlConnection = new SqlConnection(GetConnectionString()))
            {
                string query = @"DELETE FROM Transfers WHERE TransferId = @TransferId";
                var param = new { TransferId = id };
                try
                {
                    sqlConnection.Execute(query, param);
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

        public IEnumerable<Transfer> GetAll()
        {
            using (SqlConnection sqlConnection = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM Transfers";
                try
                {
                    return sqlConnection.Query<Transfer>(query);
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

        public IEnumerable<Transfer> GetByBalance(float start, float end)
        {
            using (SqlConnection sqlConnectionn = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM Transfers WHERE Balance BETWEEN @Start AND @End";
                var param = new { Start = start, End = end };
                try
                {
                    return sqlConnectionn.Query<Transfer>(query, param);
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

        public IEnumerable<Transfer> GetByDate(string date)
        {
            using (SqlConnection sqlConnection = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM Transfers WHERE Date LIKE @Date";
                var param = new { Date = '%' + date + '%' };
                try
                {
                    return sqlConnection.Query<Transfer>(query, param);
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

        public Transfer GetByTransferId(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM Transfers WHERE TransferId = @TransferId";
                var param = new { TransferId = id };
                try
                {
                    return sqlConnection.QueryFirstOrDefault<Transfer>(query, param);
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

        public IEnumerable<Transfer> GetByWalletIdFrom(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM Transfers WHERE WalletIdFrom = @WalletIdFrom";
                var param = new { WalletIdFrom = id };
                try
                {
                   return sqlConnection.Query<Transfer>(query, param);
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

        public IEnumerable<Transfer> GetByWalletIdTo(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM Transfers WHERE WalletIdTo = @WalletIdTo";
                var param = new { WalletIdTo = id };
                try
                {
                    return sqlConnection.Query<Transfer>(query, param);
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

        public void Update(Transfer obj)
        {
            using (SqlConnection sqlConnection = new SqlConnection(GetConnectionString()))
            {
                string query = @"UPDATE Transfers SET WalletIdFrom = @WalletIdFrom, WalletIdTo = @WalletIdTo, Balance = @Balance, Date = @Date WHERE TransferId = @TransferId";
                var param = new { TransferId = obj.TransferId, WalletIdFrom = obj.WalletIdFrom, WalletIdTo = obj.WalletIdTo, Balance = obj.Balance, Date = obj.Date };
                try
                {
                    sqlConnection.Execute(query, param);
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