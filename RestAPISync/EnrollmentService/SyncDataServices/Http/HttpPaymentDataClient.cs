using System;
using System.Net.Http;
using System.Net.Http.Headers;
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
        public async Task SendPostAsync(EnrollmentCreateDto post)
        {
            var myContent = JsonSerializer.Serialize(post);
            var data = new StringContent(myContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_configuration[$"PaymentService"], data);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("--> Sync POST to PaymentService was OK !");
            }
            else
            {
                Console.WriteLine("--> Sync POST to PaymentService failed");
            }
        }
    }
}