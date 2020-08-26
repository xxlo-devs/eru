using System;
using System.Collections.Generic;

namespace eru.WebApp.Models
{
    public class Status
    {
        public TimeSpan Uptime { get; set; }
        public int Subscribers { get; set; }
        public Dictionary<string, int> Classes { get; set; }
    }
}