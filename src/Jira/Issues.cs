using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Jira
{
    public class Issues
    {
        private readonly string _baseurl;
        private readonly string _token;

        public Issues(string baseurl, string token)
        {
            _baseurl = baseurl;
            _token = token;
        }

        public async Task<IEnumerable<Issue>> Query(string query)
        {
            var queryUrl =
                $"{_baseurl}/rest/tempo-rest/2.0/issues/picker/?username=&issueType=&actionType=logTime&query={query}";
           
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _token);
            var response = await client.GetStringAsync(queryUrl);
            var json = JsonConvert.DeserializeObject<RootObject>(response);
            var groups = json.group;
            if (!groups.Any())
            {
                return new List<Issue>();
            }

            var items = groups.SelectMany(group => group.item);
            return items.Select(item => new Issue(item.key, item.name));
        }

        public async Task<string> GetRemaningWork(string issue, string date, string timeSpend, string username)
        {
            var url =
                $"{_baseurl}/rest/tempo-rest/1.0/worklogs/remainingEstimate/calculate/{issue}/{date}/{date}/{timeSpend}?username={username}";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _token);
            return await client.GetStringAsync(url);
        }
    }

    public class Issue
    {
        public string Key { get; }
        public string Name { get; }

        public Issue(string key, string name)
        {
            Key = key;
            Name = name;
        }
    }

    public class Item
    {
        public string type { get; set; }
        public string key { get; set; }
        public string html { get; set; }
        public string icon { get; set; }
        public string name { get; set; }
    }

    public class Group
    {
        public string title { get; set; }
        public List<Item> item { get; set; }
    }

    public class RootObject
    {
        public List<Group> group { get; set; }
    }

}
