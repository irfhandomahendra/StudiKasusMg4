using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Data;
using PaymentService.Dtos;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentRepo _repo;
        private readonly IMapper _mapper;

        public PaymentsController(IPaymentRepo repo,
        IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<PaymentDto>> GetAllBills(string name){
            var results = await _repo.GetTagihan(name);
            var dtos = _mapper.Map<IEnumerable<PaymentDto>>(results);
            return dtos;
        }
    }
}