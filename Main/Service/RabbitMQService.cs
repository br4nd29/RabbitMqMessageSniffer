using Main.Models;
using Main.Models.RabbitMQ;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Main.Service
{
    internal  class RabbitMQService
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public int TimeOut { get; set; }
        private AuthenticationHeaderValue AuthHeader{get;set; }

        private HttpClient _client;
        private List<Task> SnifferTasks;
        public RabbitMQService(CommandLineOptions opts)
        {
            UserName = opts.Username;
            Password = opts.Password;
            Domain = opts.Domain;
            AuthHeader = new AuthenticationHeaderValue(
                "Basic",Convert.ToBase64String(
                    System.Text.ASCIIEncoding.ASCII.GetBytes(
                        $"{UserName}:{Password}")));
            TimeOut = opts.Timeout;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Authorization = AuthHeader;
            SnifferTasks = new List<Task>();
        }

       public async Task Run()
        {
            Console.WriteLine($"Go!");
            var validQueue = new List<string>();
            while (true)
            {
                (await GetQueuesAsync()).ForEach(x =>
                {
                    if (!validQueue.Contains(x.name))
                    {
                        validQueue.Add(x.name);
                        Console.WriteLine($"new queue: {x.name}");
                        var task = new Task(() => MessageSniffer(x.name));
                        task.Start();
                        SnifferTasks.Add(task);

                    }
                });

            }
        }
        private async Task<List<Queue>> GetQueuesAsync()
        {
            Thread.Sleep(TimeOut);
            var response = await _client.GetAsync($"{Domain}/api/queues/");
            return JsonConvert.DeserializeObject<Queue[]>(
                await response.Content.ReadAsStringAsync()).ToList();
        }
        private async Task MessageSniffer(object? queueName)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = AuthHeader;
                Console.WriteLine($"Start new task for: {queueName}");
                var content = new StringContent("{\"count\":100,\"ackmode\":\"ack_requeue_true\",\"encoding\":\"auto\"}");
                while (true)
                {
                    var response = await client.PostAsync(queueName.ToString(), content);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Error");
                    }
                    var body = await response.Content.ReadAsStringAsync();
                    if (body == "[]")
                    {
                        continue;
                    }
                    Console.WriteLine(body);
                }
            }
            finally
            {

                Console.WriteLine("Task Exit");
            }
        }
       
    }
}
