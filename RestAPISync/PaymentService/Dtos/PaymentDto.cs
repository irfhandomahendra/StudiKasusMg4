using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentService.Models;

namespace PaymentService.Dtos
{
    public class PaymentDto
    {
        public string Fullname { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}