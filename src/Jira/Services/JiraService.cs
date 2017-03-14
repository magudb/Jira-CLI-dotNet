using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Jira.Entities;
using Newtonsoft.Json;

namespace Jira.Services
{
    public class JiraService : IJiraService
    {
        private readonly Config _config;


        public JiraService(Config config)
        {
            _config = config;
        }

        public async Task<IEnumerable<Issue>> Query(string query)
        {
            var queryUrl =
                $"{_config.Url}/rest/tempo-rest/2.0/issues/picker/?username=&issueType=&actionType=logTime&query={query}";
           
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _config.Token());
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

        public async Task<string> GetRemaningWork(string issue, string date, string timeSpend)
        {
            var url =
                $"{_config.Url}/rest/tempo-rest/1.0/worklogs/remainingEstimate/calculate/{issue}/{date}/{date}/{timeSpend}?username={_config.Username}";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _config.Token());
            return await client.GetStringAsync(url);
        }

        public async Task LogTime(DateTime date,Issue issue, double time, string remaningTime, string comment)
        {
            var ansidate = date.ToString("yyy-MM-dd");
            var dateString = date.ToString("dd/MMM/yy").Replace("-", "/");
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("id", ""),
                new KeyValuePair<string, string>("type", ""),
                new KeyValuePair<string, string>("use-ISO8061-week-numbers", "true"),
                new KeyValuePair<string, string>("ansidate", ansidate),
                new KeyValuePair<string, string>("ansienddate", ansidate),
                new KeyValuePair<string, string>("selected-panel", "0"),
                new KeyValuePair<string, string>("analytics-origin-page", "TempoUserBoard"),
                new KeyValuePair<string, string>("analytics-origin-view", "timesheet"),
                new KeyValuePair<string, string>("analytics-origin-action", "Clicked+Log+Work+Button"),
                new KeyValuePair<string, string>("analytics-page-category", ""),
                new KeyValuePair<string, string>("startTimeEnabled", "false"),
                new KeyValuePair<string, string>("actionType", "logTime"),
                new KeyValuePair<string, string>("tracker", "false"),
                new KeyValuePair<string, string>("planning", "false"),
                new KeyValuePair<string, string>("user", _config.Username),
                new KeyValuePair<string, string>("issue", issue.Key),
                new KeyValuePair<string, string>("date", dateString),
                new KeyValuePair<string, string>("enddate", dateString),
                new KeyValuePair<string, string>("time", time.ToString(CultureInfo.InvariantCulture)),
                new KeyValuePair<string, string>("remainingEstimate", remaningTime),
                new KeyValuePair<string, string>("comment", comment),
            });

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _config.Token());
            var url = $"{_config.Url}/rest/tempo-rest/1.0/worklogs/{issue.Key}";
            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = formContent };

            var response = await client.SendAsync(request);
            var resultContent = await response.Content.ReadAsStringAsync();
            return;
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
}
