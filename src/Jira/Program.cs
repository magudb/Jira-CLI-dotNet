using System;
using Jira.Entities;
using Jira.Services;
using Microsoft.Extensions.CommandLineUtils;

namespace Jira
{
    public class Program
    {
        public static string Home {
            get
            {
                var homeDir = Environment.GetEnvironmentVariable("HOMEDRIVE");
                var homePath = Environment.GetEnvironmentVariable("HOMEPATH");
                return $"{homeDir}{homePath}";
            }
        }
        
        private static void Main(string[] args)
        {
            var app = new CommandLineApplication(false) {Name = "Jira"};
            app.HelpOption("-?|-h|--help");
            var configuration = new Configuration(Home);
            var config = Config.Read(Home).Result;
            var time = new Time(new JiraService(config));
            
            app.Command("config", configuration.ExecuteConfiguration);

            app.Command("time", time.ExecuteLog);

            app.OnExecute(() => {
                if (!config.Equals(Config.Empty())) return 0;
                Logger.Error("You forgot to run jira config");
                return 1;
            });

            app.Execute(args);
        }

       
    }
}