using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MessageSniffer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Basic", Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                "guest:guest")));
            Console.WriteLine($"Start new process for: {args[0]}");
            var content = new StringContent("{\"count\":100,\"ackmode\":\"ack_requeue_true\",\"encoding\":\"auto\"}");
            while (true)
            {
                
               var response = await client.PostAsync(args[0],content);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Error");
                }
                var kek = await response.Content.ReadAsStringAsync();
                if (kek == "[]")
                {
                    continue;
                }
                Console.WriteLine(await response.Content.ReadAsStringAsync());
               
            }
        }
    }
}
