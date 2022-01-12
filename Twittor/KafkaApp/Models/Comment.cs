using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KafkaApp.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public int TwittorId { get; set; }
        public string Message { get; set; } 
        public Twittor Twittor { get; set; }
    }
}