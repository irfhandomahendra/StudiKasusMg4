using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Models
{
    public class User
    {
        public string Username { get; set; }    
        public string Token { get; set; }
    }
}