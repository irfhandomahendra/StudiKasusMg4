using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentService.Dtos
{
    public class EnrollmentCreateDto
    {
        [Required]
        public int CourseId { get; set; }
        [Required]
        public int StudentId { get; set; }

        public float? Invoice { get; set; }
    }
}