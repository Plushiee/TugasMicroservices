using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using OrderServices.Models;
using OrderServices.DTO.Wallet;

namespace OrderServices.Services
{
    public class WalletService : IWalletService
    {
        private readonly HttpClient _httpClient;

        public WalletService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.BaseAddress = new Uri("https://localhost:7003");
        }
        public async Task<IEnumerable<Wallet>> GetAllWallet()
        {
            var response = await _httpClient.GetAsync("/walletservices/api/wallet/getall");
            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                var wallets = JsonSerializer.Deserialize<IEnumerable<Wallet>>(results);
                return wallets;
            }
            else
            {
                throw new ArgumentException($"Cannot get wallets - httpstatus : {response.StatusCode}");
            }
        }

        public async Task<Wallet> GetByWalletId(int WalletId)
        {
            var response = await _httpClient.GetAsync($"/walletservices/api/wallet/getbyid/{WalletId}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Wallet>();
                if (result == null)
                {
                    throw new ArgumentException($"Wallet with id {WalletId} not found");
                }
                else
                {
                    return result;
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new ArgumentException($"Wallet with id {WalletId} not found");
            }
            else
            {
                throw new ArgumentException($"Cannot get wallets - httpstatus : {response.StatusCode}");
            }
        }

        public async Task MinusWallet(WalletUpdateBalanceDTO walletUpdateBalanceDTO)
        {
            var requestUrl = $"/walletservices/api/wallet/minuswallet?id={walletUpdateBalanceDTO.WalletId}&balance={walletUpdateBalanceDTO.Balance}";

            var response = await _httpClient.PutAsync(requestUrl, null);

            if (!response.IsSuccessStatusCode)
            {
                throw new ArgumentException($"Cannot top up wallet - httpstatus : {response.StatusCode}");
            }
        }

        public async Task WalletTopUp(WalletUpdateBalanceDTO walletUpdateBalanceDTO)
        {
            var requestUrl = $"/walletservices/api/wallet/topupsaldo?id={walletUpdateBalanceDTO.WalletId}&balance={walletUpdateBalanceDTO.Balance}";

            var response = await _httpClient.PutAsync(requestUrl, null);

            if (!response.IsSuccessStatusCode)
            {
                throw new ArgumentException($"Cannot top up wallet - httpstatus : {response.StatusCode}");
            }
        }
    }
}