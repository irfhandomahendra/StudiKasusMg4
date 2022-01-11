using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PaymentService.Data;
using PaymentService.Dtos;
using PaymentService.Models;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/p/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private IPaymentRepo _repo;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public EnrollmentsController(IPaymentRepo repo,
        IMapper mapper, HttpClient httpClient, IConfiguration configuration)
        {
            _repo = repo;
            _mapper = mapper;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult<EnrollmentDto>> Post(EnrollmentCreateDto enrollmentCreateDto)
        {
            // var httpContent = new StringContent(
            //     jsonObject.ToString(),
            //     Encoding.UTF8,"application/json");
            // var response = await _httpClient.PostAsync(_configuration["PaymentService"],
            //     httpContent);
            await PostCallAPI(enrollmentCreateDto);
            try
            {
                var enrollment = _mapper.Map<Enrollment>(enrollmentCreateDto);
                var result = await _repo.Insert(enrollment);
                var enrollmentReturn = _mapper.Map<EnrollmentDto>(result);
                return Ok(enrollmentReturn);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        
        public async Task<object> PostCallAPI(object jsonObject)
        {
            try
            {
                var httpContent = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,"application/json");
                var response = await _httpClient.PostAsync(_configuration["PaymentService"],
                httpContent);
                if (response != null)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<object>(jsonString);
                    }
                
                // using (HttpClient client = new HttpClient())
                // {
                //     var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
                //     var response = await client.PostAsync(url, content);
                //     if (response != null)
                //     {
                //         var jsonString = await response.Content.ReadAsStringAsync();
                //         return JsonConvert.DeserializeObject<object>(jsonString);
                //     }
                // }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }
    }
}