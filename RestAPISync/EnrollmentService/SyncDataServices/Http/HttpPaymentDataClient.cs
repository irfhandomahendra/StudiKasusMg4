using System;
using System.Collections.Generic;
using System.Linq;
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

        public HttpPaymentDataClient(HttpClient httpClient,IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        public async Task SendEnrollmentToPayment(EnrollmentDto enroll)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(enroll),
                Encoding.UTF8,"application/json");
            var response = await _httpClient.PostAsync(_configuration["PaymentService"],
                httpContent);
            if(response.IsSuccessStatusCode){
                Console.WriteLine("--> Sync POST to PaymentService was OK !");
            }
            else{
                Console.WriteLine("--> Sync POST to PaymentService failed");
            }
            // using var response = await _httpClient.PostAsJsonAsync(_configuration["PaymentService"], enroll);
            // response.EnsureSuccessStatusCode();
        }
    }
}