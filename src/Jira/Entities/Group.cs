using System.Collections.Generic;

namespace Jira.Entities
{
    public class Group
    {
        public string title { get; set; }
        public List<Item> item { get; set; }
    }
}