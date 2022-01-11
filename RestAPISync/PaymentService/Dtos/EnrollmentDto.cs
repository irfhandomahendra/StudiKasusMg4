using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentService.Dtos
{
    public class EnrollmentDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public float? Invoice { get; set; }
    }
}