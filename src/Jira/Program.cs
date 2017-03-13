using System;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace Jira
{
    public class Program
    {
        
        private static void Main(string[] args)
        {
            var app = new CommandLineApplication(throwOnUnexpectedArg: false) {Name = "Jira"};
            app.HelpOption("-?|-h|--help");
            
            app.Command("config", Configuration.Config);

            app.Command("time", Time.Log);

            app.Execute(args);
        }

       
    }
}