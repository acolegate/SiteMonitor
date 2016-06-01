using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace HttpAgentManager
{
    public class Config
    {
        public enum ActionType
        {
            RunProgram,
            ShowDialog,
            WriteLine
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

        /// <summary>Initializes a new instance of the <see cref="Config"/> class.</summary>
        public Config()
        {
            Sites = new List<Site>();
        }

        public List<Site> Sites { get; }

        public class Site
        {
            public string Url { get; set; }
            public string RegexPattern { get; set; }
            public string CaptureGroupToRecord { get; set; }
            public Action ActionOnTestValueChange { get; set; }
            public TimeSpan CheckFrequency { get; set; }
            public TimeSpan Timeout { get; set; }
            public RegexTest Test { get; set; }

            public string UniqueMd5
            {
                get
                {
                    using (MD5 md5 = MD5.Create())
                    {
                        string uniqueIdentifier = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}", Url, RegexPattern, CaptureGroupToRecord, ActionOnTestValueChange.Type, ActionOnTestValueChange.Value, Test.CaptureGroup, Test.CaseSensitive, Test.TestType, Test.TestValue);

                        return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(uniqueIdentifier))).Replace("-", string.Empty);
                    }
                }
            }
        }

        public class Action
        {
            public Action(ActionType actionType, string value)
            {
                Type = actionType;
                Value = value;
            }

            public ActionType Type { get; }

            public string Value { get; }
        }

        public class RegexTest
        {
            public RegexTest(string captureGroup, TestType testType, string testValue, bool caseSensitive = false)
            {
                CaptureGroup = captureGroup;
                TestType = testType;
                TestValue = testValue;
                CaseSensitive = caseSensitive;
            }

            public string CaptureGroup { get; }
            public TestType TestType { get; }
            public string TestValue { get; }
            public bool CaseSensitive { get; }
        }
    }
}