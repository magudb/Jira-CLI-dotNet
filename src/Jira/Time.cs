using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Jira.Entities;
using Microsoft.Extensions.CommandLineUtils;

namespace Jira
{
    internal class Time
    {
        public static void Log(CommandLineApplication command)
        {
            command.Description = "Log your time";
            command.HelpOption("-?|-h|--help");
           
            var issueOption = command.Option("-i|--issue", "The issue you will be logging time on (Required)",
                CommandOptionType.SingleValue);
            var timeOption = command.Option("-t|--time", "The time to log, will default to today (Required)",
                CommandOptionType.SingleValue);
            var dateOption = command.Option("-d|--date", "The date to log time on (YYYY-MM-DD)",
                CommandOptionType.SingleValue);
            var commentOption = command.Option("-c|--comment", "the log comment, will default to 'Time logged'",
                CommandOptionType.SingleValue);

            
            command.OnExecute(async () =>
            {
                var token = await Config.Token();
                var config = await Config.Read();
                var issues = new Issues(config.Url, token);
                var hasErrors = false;
                if (!issueOption.HasValue())
                {
                    Logger.Error("You forgot to supply an issue, \\>jira --issue something --time 7,5 --date 01-02-2017");
                    hasErrors = true;
                }

                if (!timeOption.HasValue())
                {
                    Logger.Error(
                        "You forgot to supply how much time you spend, \\>jira --issue something --time 7,5 --date 01-02-2017");
                    hasErrors = true;
                }
                if (hasErrors)
                    return 1;


                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Searching for {issueOption.Value()}");
                Console.ResetColor();
                var result = (await issues.Query(issueOption.Value())).ToList();

                if (result.Count() > 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Too many Issues, Please be more precise with your issue.");
                    Console.ResetColor();
                    return 1;
                }

                var date = dateOption.HasValue() ? DateTime.Parse(dateOption.Value()) : DateTime.Now;
                
                var ansidate = date.ToString("yyy-MM-dd");
                var dateString = date.ToString("dd/MMM/yy").Replace("-", "/");

                var issue = result.FirstOrDefault();
                var remaningTime = await issues.GetRemaningWork(issue.Key, date.ToString("YYYY-MM-DD"), timeOption.Value(),
                    config.Username);
                
                var time = Double.Parse(timeOption.Value());
                var comment = commentOption.HasValue() ? commentOption.Value() : "Time logged";

               
                Logger.Information($"Logging {time.ToString(CultureInfo.InvariantCulture)} hours");
                Logger.Information($"Remaning time on {issue.Name} ({issue.Key}) : {remaningTime}");
                Logger.Information($"Using comment : {comment}");
                Logger.Information($"Using date : {ansidate} ({dateString})");

                if (!dateOption.HasValue())
                    Logger.Warning($"You did not supply date, will default to today ({dateString})");

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
                    new KeyValuePair<string, string>("user", config.Username),
                    new KeyValuePair<string, string>("issue", issue.Key),
                    new KeyValuePair<string, string>("date", dateString),
                    new KeyValuePair<string, string>("enddate", dateString),
                    new KeyValuePair<string, string>("time", time.ToString(CultureInfo.InvariantCulture)),
                    new KeyValuePair<string, string>("remainingEstimate", remaningTime),
                    new KeyValuePair<string, string>("comment", comment),
                });

                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
                var url = $"{config.Url}/rest/tempo-rest/1.0/worklogs/{issue.Key}";
                Logger.Information(url);
                var request = new HttpRequestMessage(HttpMethod.Post, url) {Content = formContent};

                var response = await client.SendAsync(request);
                var resultContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(resultContent);
               
                return 0;
            });
        }
    }
}