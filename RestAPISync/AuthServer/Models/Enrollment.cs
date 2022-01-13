using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AuthServer.Models
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public float? Invoice { get; set; }
        [JsonIgnore]
        public Course Course { get; set; }
        [JsonIgnore]
        public Student Student { get; set; }
    }
}