using System;
using Jira.Entities;
using Microsoft.Extensions.CommandLineUtils;

namespace Jira
{
    public class Configuration
    {
        private readonly string _home;

        public Configuration(string home)
        {
            _home = home;
        }
        public  void ExecuteConfiguration(CommandLineApplication command)
        {
            command.Description = "Configure your Jira Cli";
            command.HelpOption("-?|-h|--help");

           var userNameOption = command.Option("-u|--username", "Your username for Jira",
                CommandOptionType.SingleValue);
            var passwordOption = command.Option("-p|--password", "Your password for Jira",
                CommandOptionType.SingleValue);
            var urlOption = command.Option("-j|--jira-url", "The url to Jira",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                string username;
                string password;
                string url;

                if (!userNameOption.HasValue())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Please enter your Username:");
                    Console.ResetColor();
                    username = Console.ReadLine();
                }
                else
                {
                    username = userNameOption.Value();
                }

                if (!passwordOption.HasValue())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Please enter your Password:");
                    Console.ResetColor();
                    password = Console.ReadLine();
                }
                else
                {
                    password = passwordOption.Value();
                }

                if (!urlOption.HasValue())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Please enter the url for Jira:");
                    Console.ResetColor();
                    url = Console.ReadLine();
                }
                else
                {
                    url = urlOption.Value();
                }
                var config = new Config(username, password, url, _home);
                config.Write();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Your configuration has been written to {_home}/.jira-cli-config.json");
                Console.ResetColor();
                return 0;
            });

        }
    }
}
