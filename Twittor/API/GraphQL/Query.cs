using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Dtos;
using API.Kafka;
using API.Models;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace API.GraphQL
{
    public class Query
    {
        public async Task<IQueryable<TwittorDto>> GetTwittors(
            [Service] AppDbContext context,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var key = "GetTwittors-" + DateTime.Now.ToString();
            var val = JObject.FromObject(new { Message = "GraphQL Query GetTwittors" }).ToString(Formatting.None);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);
            return context.Twittors.Select(t=> new TwittorDto(){
                Id = t.Id,
                UserId = t.UserId,
                Message = t.Message,
                Created = t.Created
            });
        }

        public async Task<IQueryable<Twittor>> GetTwittorById(
           [Service] AppDbContext context,
           int id,
           [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var key = "GetTwittorById-" + DateTime.Now.ToString();
            var val = JObject.FromObject(new { Message = "GraphQL Query GetTwittorById" }).ToString(Formatting.None);

            await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);

            var twittor = context.Twittors.Include(c => c.Comments).
            Where(t => t.Id == id);

            return context.Twittors.Select(t=> new Twittor(){
                Id = t.Id,
                UserId = t.UserId,
                Message = t.Message,
                Created = t.Created,
                Comments = t.Comments
            });
        }

        public async Task<IQueryable<UserDto>> GetProfiles(
            [Service] AppDbContext context,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var key = "GetProfiles-" + DateTime.Now.ToString();
            var val = JObject.FromObject(new { Message = "GraphQL Query GetProfiles" }).ToString(Formatting.None);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);
            return context.Users.Select(p=> new UserDto(){
                Id = p.Id,
                Fullname = p.Fullname,
                Email = p.Email,
                Username = p.Username
            });
        }

    }
}