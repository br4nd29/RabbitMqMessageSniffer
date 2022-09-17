using CommandLine;

namespace Main.Models
{
    internal class CommandLineOptions
    {
        [Option(longName:"domain",Required =true,HelpText ="Message broker domain")]
        public string Domain { get; set; }
        [Option(longName:"username",Required =true)]
        public string Username {get;set;}
        [Option(longName:"password",Required =true)]
        public string Password { get; set; }
        [Option(longName:"timeout",Required = false,HelpText ="get queue timeout",Default =0)]
        public int Timeout { get; set; }
    }
}
