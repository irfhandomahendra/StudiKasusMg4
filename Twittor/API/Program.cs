using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using API.Data;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                    .AddJsonFile($"appsettings.json", true, true);

            var config = builder.Build();
            var serverConfig = new ProducerConfig
            {
                BootstrapServers = config["KafkaSettings:Server"],
                ClientId = Dns.GetHostName(),

            };
            var topics = new List<String>();
            topics.Add("Logging");
            topics.Add("User");
            topics.Add("Role");
            topics.Add("UserRole");
            topics.Add("EditProfile");
            topics.Add("ChangePassword");
            topics.Add("ChangeRole");
            topics.Add("LockUser");
            topics.Add("Twit");
            topics.Add("Comment");
            topics.Add("DeleteTwit");
            foreach (var topic in topics)
            {
                using (var adminClient = new AdminClientBuilder(serverConfig).Build())
                {
                    Console.WriteLine("Creating a topic....");
                    try
                    {
                        await adminClient.CreateTopicsAsync(new List<TopicSpecification> {
                        new TopicSpecification { Name = topic, NumPartitions = 1, ReplicationFactor = 1 } });
                    }
                    catch (CreateTopicsException e)
                    {
                        if (e.Results[0].Error.Code != ErrorCode.TopicAlreadyExists)
                        {
                            Console.WriteLine($"An error occured creating topic {topic}: {e.Results[0].Error.Reason}");
                        }
                        else
                        {
                            Console.WriteLine("Topic already exists");
                        }
                    }
                }
            }
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate(); // apply the migrations
            }
            Console.WriteLine("Database Migration");
            host.Run(); // start handling requests
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
