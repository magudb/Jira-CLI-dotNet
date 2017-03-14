using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Jira.Entities
{
    public class Config
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }


        public static async Task<Config> Read()
        {
            var stream = File.Open($"{Home}\\.jira-cli-config.json", FileMode.Open);
            var reader = new StreamReader(stream);
            var config = await reader.ReadToEndAsync();
            reader.Dispose();
            stream.Dispose();
            return JsonConvert.DeserializeObject<Config>(config);
        }

        public static void Write(string username, string password, string url)
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
    }
}