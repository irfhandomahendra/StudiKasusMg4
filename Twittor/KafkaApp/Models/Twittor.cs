using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KafkaApp.Models
{
    public class Twittor
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; }
        public User User { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}