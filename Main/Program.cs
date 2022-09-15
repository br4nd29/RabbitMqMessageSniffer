using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace Main
{
    internal class Program
    {
        public static List<string> validQueues = new List<string>();
        public static List<Thread> snifferThreads = new List<Thread>();
        public static string username { get; set; }
        public static string password { get; set; }
        public static string domain { get; set; }

        static async Task Main(string[] args)
        {
            if (args.Length ==0)
            {
                Console.WriteLine("Example: Main.exe http://localhost:15672 username password");
                Environment.Exit(0);
            }
            domain = args[0];
            username = args[1];
            password = args[2];
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                   new AuthenticationHeaderValue(
                       "Basic", Convert.ToBase64String(
                           System.Text.ASCIIEncoding.ASCII.GetBytes(
                               $"{username}:{username}")));
            while (true)
            {
               
                var response = await client.GetAsync($"{domain}/api/queues/");
                List<Models.Queue> tmp =   JsonConvert.DeserializeObject<Models.Queue[]>(await response.Content.ReadAsStringAsync()).ToList();
                tmp.ForEach(x => 
                {
                    if (!validQueues.Contains(x.name))
                    {
                        validQueues.Add(x.name);
                        Console.WriteLine($"new queue: {x.name}");
                        var thread = new Thread(MessageSniffer);
                        thread.Start($"{domain}/api/queues/%2f/{x.name}/get");
                        snifferThreads.Add(thread);
                     
                    }
                });
                response.Dispose();

            }
        }
       static async void MessageSniffer(object? queueName)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(
                            "Basic", Convert.ToBase64String(
                                System.Text.ASCIIEncoding.ASCII.GetBytes(
                                    "guest:guest")));
                Console.WriteLine($"Start new thread for: {queueName}");
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
                Console.WriteLine("Thread exit");
            }
        } 
    }
}
