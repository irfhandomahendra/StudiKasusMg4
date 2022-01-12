using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace EnrollmentService.Profiles
{
    public class EnrollmentsProfile : Profile
    {
        public EnrollmentsProfile()
        {
            CreateMap<Models.Enrollment, Dtos.EnrollmentDto>();
            CreateMap<Dtos.EnrollmentCreateDto, Models.Enrollment>();
            CreateMap<Dtos.EnrollmentDto, Models.Enrollment>();
        }
    }
}