using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnrollmentService.Dtos;
using EnrollmentService.Models;

namespace EnrollmentService.SyncDataServices.Http
{
    public interface IPaymentDataClient
    {
        Task<object> SendEnrollmentToPayment (object jsonObject);
        Task SendPostAsync(EnrollmentDto post);
    }
}