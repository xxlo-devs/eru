using System;
using System.Collections.Generic;
using System.Text;

namespace eru.Application.Tests
{
    public static class MockData
    {
        public const string CorrectIpAddress = "198.51.100.1";
        public const string CorrectUploadKey = "sample-key";
        public static DateTime CorrectDate { get; } = new DateTime(2010, 1, 1);
        public const string ExistingClassId = "sample-class-id";
        public const string ExistingUserId = "sample-user";
    }
}
