using System;

namespace eru.Domain.Entity
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string NormalizedUsername { get; set; }
        public string PasswordHash { get; set; }
        public int AccessFailed { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEndDate { get; set; }
    }
}