using System;
using System.Globalization;
using System.Linq;
using Jira.Entities;
using Jira.Services;
using Microsoft.Extensions.CommandLineUtils;

namespace Jira
{
    public class Time
    {
       
        private readonly IJiraService _service;

        public Time(IJiraService service)
        {
           
            _service = service;
        }

        public void ExecuteLog(CommandLineApplication command)
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
                var result = (await _service.Query(issueOption.Value())).ToList();

                if (result.Count() > 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Too many Issues, Please be more precise with your issue.");
                    Console.ResetColor();
                    return 1;
                }

                var date = dateOption.HasValue() ? DateTime.Parse(dateOption.Value()) : DateTime.Now;


                var issue = result.FirstOrDefault();
                var remaningTime = await _service.GetRemaningWork(issue.Key, date.ToString("YYYY-MM-DD"),
                    timeOption.Value());

                var time = double.Parse(timeOption.Value());
                var comment = commentOption.HasValue() ? commentOption.Value() : "Time logged";


                Logger.Information($"Logging {time.ToString(CultureInfo.InvariantCulture)} hours");
                Logger.Information($"Remaning time on {issue.Name} ({issue.Key}) : {remaningTime}");
                Logger.Information($"Using comment : {comment}");

                if (!dateOption.HasValue())
                    Logger.Warning($"You did not supply date, will default to today ({date})");


                await _service.LogTime(date, issue, time, remaningTime, comment);

                return 0;
            });
        }
    }
}