using System;
using System.Collections.Generic;

namespace eru.WebApp.Areas.AdminDashboard.Model
{
    public class Status
    {
        public TimeSpan Uptime { get; set; }
        public int Subscribers { get; set; }
        public Dictionary<string, int> Classes { get; set; }
    }
}