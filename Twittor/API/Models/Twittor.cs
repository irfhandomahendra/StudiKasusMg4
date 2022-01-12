using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Twittor
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}