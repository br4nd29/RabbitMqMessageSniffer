using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using Main.Models.RabbitMQ;
using CommandLine;
using Main.Models;
using Main.Service;

namespace Main
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
             await Parser.Default.ParseArguments<CommandLineOptions>(args)
                 .MapResult(async (CommandLineOptions opts) =>
                 {
                     try
                     {
                         var service = new RabbitMQService(opts);
                         await service.Run();
                         return 0;
                     }
                     catch (Exception)
                     {

                         Console.WriteLine("Error!");
                         return -3;
                     }
                 },
                 errs => Task.FromResult(-1)); 
        }
     
    }
}
