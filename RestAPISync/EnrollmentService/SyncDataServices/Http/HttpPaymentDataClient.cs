using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EnrollmentService.Dtos;
using Microsoft.Extensions.Configuration;

namespace EnrollmentService.SyncDataServices.Http
{
    public class HttpPaymentDataClient : IPaymentDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HttpPaymentDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        public async Task<object> SendEnrollmentToPayment(object jsonObject)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(jsonObject).ToString(),
                Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsJsonAsync(_configuration["PaymentService"],
                httpContent);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("--> Sync POST to PaymentService was OK !");
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<object>(jsonString);
                Console.WriteLine(result);
                return result;
            }
            else
            {
                Console.WriteLine("--> Sync POST to PaymentService failed");
            }
            // using var response = await _httpClient.PostAsJsonAsync(_configuration["PaymentService"], enroll);
            // response.EnsureSuccessStatusCode();
            return null;
        }

        public async Task SendPostAsync(EnrollmentDto post)
        {
            using var response = await _httpClient.PostAsJsonAsync("https://localhost:6001/api/p/enrollments", post);

            response.EnsureSuccessStatusCode();
        }
    }
}