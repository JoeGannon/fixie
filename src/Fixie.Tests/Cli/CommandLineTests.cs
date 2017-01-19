﻿namespace Fixie.Tests.Cli
{
    using Assertions;
    using Fixie.Cli;

    public class CommandLineTests
    {
        class Empty { }

        public void ShouldParseEmptyModels()
        {
            Parse<Empty>().ShouldSucceed(new Empty());

            Parse<Empty>("first", "second", "third", "fourth", "fifth")
                .ShouldSucceed(new Empty(), "first", "second", "third", "fourth", "fifth");
        }

        static Scenario<T> Parse<T>(params string[] arguments) where T : class
        {
            return new Scenario<T>(arguments);
        }

        class Scenario<T> where T : class
        {
            readonly string[] arguments;

            public Scenario(params string[] arguments)
            {
                this.arguments = arguments;
            }

            public void ShouldSucceed(T expectedModel, params string[] expectedUnusedArguments)
            {
                string[] unusedArguments;
                var model = CommandLine.Parse<T>(arguments, out unusedArguments);
                ShouldMatch(model, expectedModel);
                unusedArguments.ShouldEqual(expectedUnusedArguments);
            }

            static void ShouldMatch(T actual, T expected)
            {
                expected.ShouldNotBeNull();
                actual.ShouldNotBeNull();

                foreach (var property in typeof(T).GetProperties())
                {
                    var actualValue = property.GetValue(actual);
                    var expectedValue = property.GetValue(expected);

                    actualValue.ShouldEqual(expectedValue);
                }
            }
        }
    }
}