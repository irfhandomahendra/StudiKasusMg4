using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
        public async Task<ActionResult<EnrollmentDto>> Post([FromBody] EnrollmentCreateDto enrollmentCreateDto){
            try
            {
                var enrollment = _mapper.Map<Enrollment>(enrollmentCreateDto);
                var result = await _repo.Insert(enrollment);
                Console.WriteLine("--> Enrollment succesfully added!");
                return Ok(_mapper.Map<EnrollmentDto>(result));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);                
            }
        }
    }
}