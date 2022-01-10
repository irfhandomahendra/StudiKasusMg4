using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnrollmentService.Models;

namespace EnrollmentService.Data
{
    public class StudentDAL : IStudent
    {
        private AppDbContext _db;

        public StudentDAL(AppDbContext db)
        {
            _db = db;
        }
        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Student>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Student> GetById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Student> Insert(Student obj)
        {
            throw new NotImplementedException();
        }

        public Task<Student> Update(string id, Student obj)
        {
            throw new NotImplementedException();
        }
    }
}