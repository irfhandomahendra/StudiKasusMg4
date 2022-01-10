using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnrollmentService.Dtos
{
    public class StudentCreateDto
    {
        [Required(ErrorMessage = "Kolom Fullname harus diisi")]
        [MaxLength(20, ErrorMessage = "Tidak boleh lebih dari 20 karakter")]
        public string Fullname { get; set; }

        [Required]
        public DateTime EnrollmentDate { get; set; }
    }
}