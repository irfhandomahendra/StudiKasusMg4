using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EnrollmentService.Data;
using EnrollmentService.Dtos;
using EnrollmentService.Models;
using EnrollmentService.SyncDataServices.Http;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private IEnrollment _enrollment;
        private IMapper _mapper;
        private readonly IPaymentDataClient _paymentDataClient;

        public EnrollmentsController(IEnrollment enrollment, IMapper mapper,
        IPaymentDataClient paymentDataClient)
        {
            _enrollment = enrollment;
            _mapper = mapper;
            _paymentDataClient = paymentDataClient;
        }

        // GET: api/<EnrollmentsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> Get()
        {
            var enrollments = await _enrollment.GetAll();
            var dtos = _mapper.Map<IEnumerable<EnrollmentDto>>(enrollments);
            return Ok(dtos);
        }

        // GET api/<EnrollmentsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EnrollmentDto>> GetEnrollmentById(int id)
        {
            var result = await _enrollment.GetById(id.ToString());
            if (result == null)
                return NotFound();

            return Ok(_mapper.Map<EnrollmentDto>(result));
        }


        // POST api/<EnrollmentsController>
        [HttpPost]
        public async Task<ActionResult<EnrollmentDto>> Post([FromBody] EnrollmentCreateDto enrollmentCreateDto)
        {
            var enrollment = _mapper.Map<Enrollment>(enrollmentCreateDto);
            //var result = await _enrollment.Insert(enrollment);
            var dtos = _mapper.Map<EnrollmentDto>(enrollment);

            try
            {
                await _paymentDataClient.SendEnrollmentToPayment(dtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetEnrollmentById),dtos);
        }

        // PUT api/<EnrollmentsController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<EnrollmentDto>> Put(int id, [FromBody] EnrollmentCreateDto enrollmentCreateDto)
        {
            try
            {
                var enrollment = _mapper.Map<Models.Enrollment>(enrollmentCreateDto);
                var result = await _enrollment.Update(id.ToString(), enrollment);
                var dtos = _mapper.Map<Dtos.EnrollmentDto>(result);
                return Ok(dtos);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<EnrollmentsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _enrollment.Delete(id.ToString());
                return Ok($"Data enrollment {id} berhasil di delete");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}