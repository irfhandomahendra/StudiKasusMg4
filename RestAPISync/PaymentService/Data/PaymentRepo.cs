using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentService.Models;

namespace PaymentService.Data
{
    public class PaymentRepo : IPaymentRepo
    {
        private AppDbContext _db;

        public PaymentRepo(AppDbContext db)
        {
            _db = db;
        }
        public async Task<Enrollment> GetById(int id)
        {
            var result = await (from c in _db.Enrollments
                                where c.Id == id
                                select c).SingleOrDefaultAsync();
            if (result == null) throw new Exception($"Data id {id} tidak ditemukan !");
           
            return result;
        }

        public async Task<IEnumerable<Student>> GetTagihan(string name)
        {
            var results = await(from a in _db.Students.Include(e => e.Enrollments)
                                where a.Fullname.ToLower().Contains(name.ToLower())
                                orderby a.Id ascending
                                select a).AsNoTracking().ToListAsync();
            return results;
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
    }
}