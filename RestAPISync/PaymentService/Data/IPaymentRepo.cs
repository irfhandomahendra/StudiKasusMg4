using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentService.Models;

namespace PaymentService.Data
{
    public interface IPaymentRepo
    {
        Task<Enrollment> GetById(int id);
        Task<Enrollment> Insert(Enrollment obj);
        Task<IEnumerable<Student>> GetTagihan(string name);
    }
}