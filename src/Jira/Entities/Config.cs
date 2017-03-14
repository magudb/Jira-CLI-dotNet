using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Jira.Entities
{
    public class Config
    {
        
        public Config(string username, string password, string url, string home)
        {
            Username = username;
            Password = password;
            Url = url;
            Home = home;
        }

        public string Username { get;  }
        public string Password { get;  }
        public string Url { get;  }
        public string Home { get; }


        public static async Task<Config> Read(string home)
        {
            var file = $"{home}\\.jira-cli-config.json";
            if (!File.Exists(file))
            {
                return Config.Empty();
            }
            var stream = File.Open(file, FileMode.Open);
            var reader = new StreamReader(stream);
            var config = await reader.ReadToEndAsync();
            reader.Dispose();
            stream.Dispose();
            return JsonConvert.DeserializeObject<Config>(config);
        }

        public static Config Empty()
        {
            return  new Config(string.Empty, string.Empty, string.Empty, string.Empty);
        }

        public void Write()
        {
           
            var configString = JsonConvert.SerializeObject(this);
            var configFile = File.Create($"{Home}\\.jira-cli-config.json");
            var logWriter = new StreamWriter(configFile);
            logWriter.Write(configString);
            logWriter.Dispose();
            configFile.Dispose();
        }

        

        public string Token()
        {
            return Base64Encode($"{Username}:{Password}");
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}