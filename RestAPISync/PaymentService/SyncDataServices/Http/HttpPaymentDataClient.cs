using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PaymentService.Dtos;

namespace PaymentService.SyncDataServices.Http
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
        public async Task<EnrollmentDto> PostCallAPI(EnrollmentDto jsonObject)
        {
            // try
            // {
            //     var httpContent = new StringContent(
            //     jsonObject.ToString(),
            //     Encoding.UTF8,"application/json");
            //     var response = await _httpClient.PostAsync(_configuration["PaymentService"],
            //     httpContent);
            //     if (response != null)
            //         {
            //             var jsonString = await response.Content.ReadAsStreamAsync();
            //             return await JsonSerializer.DeserializeAsync<EnrollmentDto>(jsonString, new System.Text.Json.JsonSerializerOptions { IgnoreNullValues = true, PropertyNameCaseInsensitive = true });
            //         }

            //     // using (HttpClient client = new HttpClient())
            //     // {
            //     //     var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            //     //     var response = await client.PostAsync(url, content);
            //     //     if (response != null)
            //     //     {
            //     //         var jsonString = await response.Content.ReadAsStringAsync();
            //     //         return JsonConvert.DeserializeObject<object>(jsonString);
            //     //     }
            //     // }
            // }
            // catch (Exception)
            // {
            //     return null;
            // }
            // return null;

            // JsonSerializerOptions jsonSerializerOptions
            // = new JsonSerializerOptions
            // {
            //     PropertyNameCaseInsensitive = true,
            //     PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            // };

            // using var response = await _httpClient.GetAsync(_configuration["PaymentService"]);
            // response.EnsureSuccessStatusCode();

            // using var contentStream = await response.Content.ReadAsStreamAsync();

            // var posts = await JsonSerializer.DeserializeAsync<EnrollmentDto>
            //     (contentStream, jsonSerializerOptions);

            // return posts;



            var post = await _httpClient.GetFromJsonAsync<EnrollmentDto>(_configuration["PaymentService"]);
            return post;
        }

        public async Task<IEnumerable<EnrollmentDto>> GetPostsAsync()
        {
            var posts = await _httpClient.GetFromJsonAsync<IEnumerable<EnrollmentDto>>("https://localhost:6001/api/p/enrollments");

            return posts;
        }
    }
}