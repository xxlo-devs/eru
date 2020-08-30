using System;
using System.Collections.Generic;

namespace eru.WebApp.Models
{
    public class Status
    {
        public TimeSpan Uptime { get; set; }
        public int Subscribers { get; set; }
        public IEnumerable<ClassInfo> Classes { get; set; }
    }

    public class ClassInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int SubscribersCount { get; set; }
    }
}