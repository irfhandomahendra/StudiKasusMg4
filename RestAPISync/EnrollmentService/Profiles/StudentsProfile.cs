using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace EnrollmentService.Profiles
{
    public class StudentsProfile : Profile
    {
        public StudentsProfile()
        {
            CreateMap<Models.Student, Dtos.StudentDto>();
            CreateMap<Dtos.StudentCreateDto, Models.Student>();
        }
    }
}