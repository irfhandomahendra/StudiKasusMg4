using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnrollmentService.Models;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentService.Data
{
    public class EnrollmentDAL : IEnrollment
    {
        private AppDbContext _db;

        public EnrollmentDAL(AppDbContext db)
        {
            _db = db;
        }
        public async Task Delete(string id)
        {
             try
            {
                var result = await GetById(id);
                if (result == null) throw new Exception($"Data course {id} tidak ditemukan !");
                _db.Enrollments.Remove(result);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"Error: {dbEx.Message}");
            }
        }

        public async Task<IEnumerable<Enrollment>> GetAll()
        {
            var results = await _db.Enrollments.Include(e => e.Student)
                .Include(e=>e.Course).AsNoTracking().ToListAsync();
            return results;
        }

        public async Task<Enrollment> GetById(string id)
        {
            var result = await (from c in _db.Enrollments
                                where c.Id == Convert.ToInt32(id)
                                select c).SingleOrDefaultAsync();
            if (result == null) throw new Exception($"Data id {id} tidak ditemukan !");
           
            return result;
        }

        public async Task<Enrollment> Insert(Enrollment obj)
        {
            try
            {
                _db.Enrollments.Add(obj);
                await _db.SaveChangesAsync();
                return obj;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"Error: {dbEx.Message}");
            }
        }

        public async Task<Enrollment> Update(string id, Enrollment obj)
        {
            try
            {
                var result = await GetById(id);
                if (result == null) throw new Exception($"data course id {id} tidak ditemukan");
                result.CourseId = obj.CourseId;
                result.StudentId = obj.StudentId;
                result.Invoice = obj.Invoice;
                await _db.SaveChangesAsync();
                return result;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"Error: {dbEx.Message}");
            }
        }
    }
}