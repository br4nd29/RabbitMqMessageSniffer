using CommandLine;
using CommandLine.Text;

using System.Collections.Generic;

namespace Main.Models
{
    internal class CommandLineOptions
    {
        
        [Option(longName:"domain",shortName:'d',Required =true,HelpText ="Message broker domain")]
        public string Domain { get; set; }
        [Option(longName:"username",shortName:'u',Required =true)]
        public string Username {get;set;}
        [Option(longName:"password",shortName:'p',Required =true)]
        public string Password { get; set; }
        [Option(longName:"timeout",shortName:'t',Required = false,HelpText ="get queue timeout",Default =0)]
        public int Timeout { get; set; }
        [Usage(ApplicationAlias = ".\\RabbitMQSniffer.exe")]
        public static IEnumerable<Example> Examples 
        {
            get
            {
                yield return new Example("Default", new CommandLineOptions { Domain = "http://localhost:15672", Username = "guest", Password = "guest" });
            }
        }
       
    }
}
