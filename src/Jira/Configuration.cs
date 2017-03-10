using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace Jira
{
    public class Configuration
    {
        public static string Home
        {
            get
            {
                var homeDir = Environment.GetEnvironmentVariable("HOMEDRIVE");
                var homePath = Environment.GetEnvironmentVariable("HOMEPATH");
                return $"{homeDir}{homePath}";
            }
        }

        public static async Task<string> Token()
        {
            var config = await Read();
            return Base64Encode($"{config.Username}:{config.Password}");
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
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
                var username = "";
                var password = "";
                var url = "";
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
                Write(username, password, url);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Your configuration has been written to {Home}/.jira-cli-config.json");
                Console.ResetColor();
                return 0;
            });

        }

        public static async Task<Config> Read()
        {
            var stream = File.Open($"{Home}\\.jira-cli-config.json", FileMode.Open);
            var reader = new StreamReader(stream);
            var config = await reader.ReadToEndAsync();
            reader.Dispose();
            stream.Dispose();
            return JsonConvert.DeserializeObject<Config>(config);
        }

        private static void Write(string username, string password, string url)
        {
            var config = new Config
            {
                Username = username,
                Password = password,
                Url = url
            };
            var configString = JsonConvert.SerializeObject(config);
            var configFile = File.Create($"{Home}\\.jira-cli-config.json");
            var logWriter = new StreamWriter(configFile);
            logWriter.Write(configString);
            logWriter.Dispose();
            configFile.Dispose();
        }
    }

    public class Config
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
    }
}
