using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using HotChocolate;

namespace API.GraphQL
{
    public class Query
    {
        public IQueryable<Twittor> GetTwittors([Service] AppDbContext context) =>
            context.Twittors;
    }
}