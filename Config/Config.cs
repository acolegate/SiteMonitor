using System;
using System.Collections.Generic;

namespace Config
{
    public class Config
    {
        public enum ActionType
        {
            RunProgram,
            ShowDialog,
            WriteLine
        }

        public enum RegexTestsOperator
        {
            And,
            Or
        }

        public enum TestType
        {
            Equals,
            Contains,
            StartsWith,
            EndsWith,
            IsNotEqualTo,
            DoesNotContain,
            DoesNotStartWith,
            DoesNotEndWith
        }

        public Config()
        {
            Sites = new List<Site>();
        }

        public List<Site> Sites { get; }

        public class Site
        {
            public string Url { get; set; }
            public string RegexPattern { get; set; }
            public List<RegexTest> Tests { get; set; }
            public RegexTestsOperator? Operator { get; set; }
            public Action ActionOnMatch { get; set; }
            public TimeSpan CheckFrequency { get; set; }
            public TimeSpan Timeout { get; set; }
        }

        public class Action
        {
            public ActionType Type { get; set; }
            public string Value { get; set; }
        }

        public class RegexTest
        {
            public RegexTest(string captureGroup, TestType testType, string testValue )
            {
                CaptureGroup = captureGroup;
                TestType = testType;
                TestValue = testValue;
            }

            public string CaptureGroup { get; set; }
            public TestType TestType { get; set; }
            public string TestValue { get; set; }
        }
    }
}