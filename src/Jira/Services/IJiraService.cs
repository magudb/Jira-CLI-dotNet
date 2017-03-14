using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jira.Entities;

namespace Jira.Services
{
    public interface IJiraService
    {
        Task<IEnumerable<Issue>> Query(string query);
        Task<string> GetRemaningWork(string issue, string date, string timeSpend);
        Task LogTime(DateTime date, Issue issue, double time, string remaningTime, string comment);
    }
}