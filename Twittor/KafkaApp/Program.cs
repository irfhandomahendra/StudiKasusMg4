using System;
using System.Collections.Generic;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using KafkaApp.Data;
using KafkaApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KafkaApp
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                    .AddJsonFile($"appsettings.json", true, true);

            var config = builder.Build();


            var Serverconfig = new ConsumerConfig
            {
                BootstrapServers = config["Settings:KafkaServer"],
                GroupId = "tester",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };
            Console.WriteLine("--------------.NET Application--------------");
            using (var consumer = new ConsumerBuilder<string, string>(Serverconfig).Build())
            {
                Console.WriteLine("Connected");
                var topics = new string[] { "User", "Role", "UserRole", "EditProfile" , "ChangePassword",
                "ChangeRole", "LockUser", "Twit", "Comment", "DeleteTwit" };
                consumer.Subscribe(topics);

                Console.WriteLine("Waiting messages....");
                try
                {
                    while (true)
                    {
                        var cr = consumer.Consume(cts.Token);
                        Console.WriteLine($"Consumed record with Topic: {cr.Topic} key: {cr.Message.Key} and value: {cr.Message.Value}");

                        using (var dbcontext = new AppDbContext())
                        {
                            if (cr.Topic == "User")
                            {
                                User user = JsonConvert.DeserializeObject<User>(cr.Message.Value);
                                dbcontext.Users.Add(user);
                            }
                            if (cr.Topic == "Role")
                            {
                                Role role = JsonConvert.DeserializeObject<Role>(cr.Message.Value);
                                dbcontext.Roles.Add(role);
                            }
                            if (cr.Topic == "UserRole")
                            {
                                UserRole userRole = JsonConvert.DeserializeObject<UserRole>(cr.Message.Value);
                                dbcontext.UserRoles.Add(userRole);
                            }
                            if (cr.Topic == "EditProfile")
                            {
                                User prof = JsonConvert.DeserializeObject<User>(cr.Message.Value);
                                dbcontext.Users.Update(prof);
                            }
                            if (cr.Topic == "ChangePassword")
                            {
                                User pass = JsonConvert.DeserializeObject<User>(cr.Message.Value);
                                dbcontext.Users.Update(pass);
                            }
                            if (cr.Topic == "ChangeRole")
                            {
                                UserRole userRole = JsonConvert.DeserializeObject<UserRole>(cr.Message.Value);
                                dbcontext.UserRoles.Update(userRole);
                            }
                            if (cr.Topic == "LockUser")
                            {
                                UserRole userRole = JsonConvert.DeserializeObject<UserRole>(cr.Message.Value);
                                dbcontext.UserRoles.Remove(userRole);
                            }
                            if (cr.Topic == "Twit")
                            {
                                Twittor twit = JsonConvert.DeserializeObject<Twittor>(cr.Message.Value);
                                dbcontext.Twittors.Add(twit);
                            }
                            if (cr.Topic == "Comment")
                            {
                                Comment comment = JsonConvert.DeserializeObject<Comment>(cr.Message.Value);
                                dbcontext.Comments.Add(comment);
                            }
                            if (cr.Topic == "DeleteTwit")
                            {
                                Twittor twit = JsonConvert.DeserializeObject<Twittor>(cr.Message.Value);
                                dbcontext.Twittors.Remove(twit);
                            }
                            await dbcontext.SaveChangesAsync();
                            Console.WriteLine("Data was saved into database");
                        }


                    }
                }
                catch (OperationCanceledException)
                {
                    // Ctrl-C was pressed.
                }
                finally
                {
                    consumer.Close();
                }

            }

            return 1;
        }
    }
}
