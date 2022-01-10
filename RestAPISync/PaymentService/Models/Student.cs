using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentService.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Fullname { get; set; }
        
        public DateTime EnrollmentDate { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}