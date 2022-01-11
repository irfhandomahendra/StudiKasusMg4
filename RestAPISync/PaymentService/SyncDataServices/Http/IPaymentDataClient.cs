using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentService.Dtos;

namespace PaymentService.SyncDataServices.Http
{
    public interface IPaymentDataClient
    {
        public Task<EnrollmentDto> PostCallAPI(EnrollmentDto jsonObject);
    }
}