using System;

namespace HttpAgentManager
{
    public static class HelperFunctions
    {
        public static bool EvaluateTest(Config.RegexTest test, string captureGroupValue)
        {
            StringComparison stringComparison = test.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            switch (test.TestType)
            {
                case Config.TestType.Contains:
                    {
                        return captureGroupValue.Contains(test.TestValue);
                    }
                case Config.TestType.Equals:
                    {
                        return captureGroupValue.Equals(test.TestValue, stringComparison);
                    }
                case Config.TestType.StartsWith:
                    {
                        return captureGroupValue.StartsWith(test.TestValue, stringComparison);
                    }
                case Config.TestType.EndsWith:
                    {
                        return captureGroupValue.EndsWith(test.TestValue, stringComparison);
                    }
                case Config.TestType.DoesNotContain:
                    {
                        return captureGroupValue.Contains(test.TestValue) == false;
                    }
                case Config.TestType.IsNotEqualTo:
                    {
                        return captureGroupValue.Equals(test.TestValue, stringComparison) == false;
                    }
                case Config.TestType.DoesNotStartWith:
                    {
                        return captureGroupValue.StartsWith(test.TestValue, stringComparison) == false;
                    }
                case Config.TestType.DoesNotEndWith:
                    {
                        return captureGroupValue.EndsWith(test.TestValue, stringComparison) == false;
                    }

                default:
                    {
                        return false;
                    }
            }
        }
    }
}