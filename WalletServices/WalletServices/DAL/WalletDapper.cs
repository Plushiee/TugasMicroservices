using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletServices.Models;
using Dapper;
using WalletServices.DAL.Interfaces;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WalletServices.DAL
{
    public class WalletDapper : IWallet
    {
        private string GetConnectionString()
        {
            return "Data Source=.\\SQLEXPRESS; Initial Catalog=WalletDB;Integrated Security=true;TrustServerCertificate=false;";
        }

        public Wallet Add(Wallet obj)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"INSERT INTO Wallets (Username, Email, FullName, Balance) VALUES (@Username, @Email, @FullName, @Balance); SELECT CAST(SCOPE_IDENTITY() as int)";
                var param = new { Username = obj.Username, Email = obj.Email, FullName = obj.FullName, Balance = obj.Balance };
                try
                {
                    int newWalletId = conn.ExecuteScalar<int>(query, param);

                    obj.WalletId = newWalletId;

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
                string query = @"DELETE FROM Wallets WHERE WalletId = @WalletId";
                var param = new { WalletId = id };
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

        public IEnumerable<Wallet> GetAll()
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM Wallets";
                try
                {
                    return conn.Query<Wallet>(query);
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

        public IEnumerable<Wallet> GetByEmail(string email)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM Wallets WHERE Email LIKE @Email";
                var param = new { Email = '%' + email + '%' };
                try
                {
                    return conn.Query<Wallet>(query, param);
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

        public IEnumerable<Wallet> GetByFullName(string fullname)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM Wallets WHERE FullName LIKE @FullName";
                var param = new { FullName = '%' + fullname + '%' };
                try
                {
                    return conn.Query<Wallet>(query, param);
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

        public IEnumerable<Wallet> GetByUsername(string username)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM Wallets WHERE Username LIKE @Username";
                var param = new { Username = '%' + username + '%' };
                try
                {
                    return conn.Query<Wallet>(query, param);
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

        public void Update(Wallet obj)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"UPDATE Wallets SET Username = @Username, Password = @Password, Email = @Email, FullName = @FullName, Balance = @Balance WHERE WalletId = @WalletId";
                var param = new { WalletId = obj.WalletId, Username = obj.Username, Password = obj.Password, Email = obj.Email, FullName = obj.FullName, Balance = obj.Balance };
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

        public Wallet GetByWalletId(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM Wallets WHERE WalletId = @WalletId";
                var param = new { WalletId = id };
                try
                {
                    return conn.QueryFirstOrDefault<Wallet>(query, param);
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

        public IEnumerable<Wallet> GetByBalance(float start, float end)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM Wallets WHERE Balance BETWEEN @Start AND @End";
                var param = new { Start = start, End = end };
                try
                {
                    return conn.Query<Wallet>(query, param);
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

        public Wallet TopUpWallet(int walletId, float balance)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"UPDATE Wallets SET Balance = Balance + @Balance WHERE WalletId = @WalletId";
                var param = new { WalletId = walletId, Balance = balance };
                try
                {
                    return conn.QueryFirstOrDefault<Wallet>(query, param);
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

        public Wallet MinusWallet(int walletId, float balance)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"UPDATE Wallets SET Balance = Balance - @Balance WHERE WalletId = @WalletId";
                var param = new { WalletId = walletId, Balance = balance };
                try
                {
                    return conn.QueryFirstOrDefault<Wallet>(query, param);
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