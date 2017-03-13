using System;
using Microsoft.Extensions.CommandLineUtils;

namespace Jira
{
    public class Configuration
    {
        public static void Config(CommandLineApplication command)
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
                Jira.Config.Write(username, password, url);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Your configuration has been written to {Jira.Config.Home}/.jira-cli-config.json");
                Console.ResetColor();
                return 0;
            });

        }
    }
}
