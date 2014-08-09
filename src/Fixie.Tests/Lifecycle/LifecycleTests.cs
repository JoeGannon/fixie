﻿using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Should;

namespace Fixie.Tests.Lifecycle
{
    public abstract class LifecycleTests
    {
        static string[] FailingMembers;
        protected readonly Convention Convention;

        protected LifecycleTests()
        {
            FailingMembers = null;

            Convention = new Convention();
            Convention.Classes.Where(testClass => testClass == typeof(SampleTestClass));
            Convention.Methods.Where(method => method.Name == "Pass" || method.Name == "Fail");
        }

        protected static void FailDuring(params string[] failingMemberNames)
        {
            FailingMembers = failingMemberNames;
        }

        protected Output Run()
        {
            using (var console = new RedirectedConsole())
            {
                var listener = new StubListener();

                Convention.ClassExecution.SortCases((x, y) => String.Compare(y.Name, x.Name, StringComparison.Ordinal));

                typeof(SampleTestClass).Run(listener, Convention);

                return new Output(console.Lines().ToArray(), listener.Entries.ToArray());
            }
        }

        protected class Output
        {
            readonly string[] lifecycle;
            readonly string[] results;

            public Output(string[] lifecycle, string[] results)
            {
                this.lifecycle = lifecycle;
                this.results = results;
            }

            public void ShouldHaveLifecycle(params string[] expected)
            {
                lifecycle.ShouldEqual(expected);
            }

            public void ShouldHaveResults(params string[] expected)
            {
                var namespaceQualifiedExpectation = expected.Select(x => "Fixie.Tests.Lifecycle.LifecycleTests+" + x).ToArray();

                results.ShouldEqual(namespaceQualifiedExpectation);
            }
        }

        protected class SampleTestClass : IDisposable
        {
            bool disposed;

            public SampleTestClass()
            {
                WhereAmI();
            }

            public void SetUpA()
            {
                WhereAmI();
            }

            public void SetUpB()
            {
                WhereAmI();
            }

            public void Pass()
            {
                WhereAmI();
            }

            public void Fail()
            {
                WhereAmI();
                throw new FailureException();
            }

            public void TearDownA()
            {
                WhereAmI();
            }

            public void TearDownB()
            {
                WhereAmI();
            }

            public void Dispose()
            {
                if (disposed)
                    throw new ShouldBeUnreachableException();
                disposed = true;

                WhereAmI();
            }
        }

        protected static void ClassSetUp(ClassExecution classExecution)
        {
            classExecution.Type.ShouldEqual(typeof(SampleTestClass));
            WhereAmI();
        }

        protected static void ClassTearDown(ClassExecution classExecution)
        {
            classExecution.Type.ShouldEqual(typeof(SampleTestClass));
            WhereAmI();
        }

        protected static void InstanceSetUp(InstanceExecution instanceExecution)
        {
            instanceExecution.TestClass.ShouldEqual(typeof(SampleTestClass));
            WhereAmI();
        }

        protected static void InstanceTearDown(InstanceExecution instanceExecution)
        {
            instanceExecution.TestClass.ShouldEqual(typeof(SampleTestClass));
            WhereAmI();
        }

        protected static void CaseSetUp(Case @case)
        {
            @case.Class.ShouldEqual(typeof(SampleTestClass));
            @case.Instance.ShouldBeType<SampleTestClass>();
            WhereAmI();
        }

        protected static void CaseTearDown(Case @case)
        {
            @case.Class.ShouldEqual(typeof(SampleTestClass));
            @case.Instance.ShouldBeType<SampleTestClass>();
            WhereAmI();
        }

        protected static void WhereAmI([CallerMemberName] string member = null)
        {
            Console.WriteLine(member);

            if (FailingMembers != null && FailingMembers.Contains(member))
                throw new FailureException(member);
        }

        protected class CaseSetUpTearDown : CaseBehavior
        {
            public void Execute(Case @case, Action next)
            {
                CaseSetUp(@case);
                next();
                CaseTearDown(@case);
            }
        }

        protected class InstanceSetUpTearDown : InstanceBehavior
        {
            public void Execute(InstanceExecution instanceExecution, Action next)
            {
                InstanceSetUp(instanceExecution);
                next();
                InstanceTearDown(instanceExecution);
            }
        }

        protected class ClassSetUpTearDown : ClassBehavior
        {
            public void Execute(ClassExecution classExecution, Action next)
            {
                ClassSetUp(classExecution);
                next();
                ClassTearDown(classExecution);
            }
        }
    }
}