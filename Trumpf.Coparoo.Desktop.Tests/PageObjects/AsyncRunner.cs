// Copyright 2016 - 2023 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Trumpf.Coparoo.Desktop.Tests.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Coparoo.Desktop;
    using NUnit.Framework;
    using PageTests;
    using WinForms;

    /// <summary>
    /// Tests for the asynchronous page test runner. A separate root object is used, so that the page tests
    /// defined here do not interfere with the page tests of the <see cref="Runner"/> fixture.
    /// </summary>
    [TestFixture]
    public class AsyncRunner
    {
        private static readonly object Lock = new object();
        private static readonly List<string> Log = new List<string>();

        private static void Add(string entry)
        {
            lock (Lock)
            {
                Log.Add(entry);
            }
        }

        private static string[] Entries()
        {
            lock (Lock)
            {
                return Log.ToArray();
            }
        }

        [SetUp]
        public void SetUp()
        {
            lock (Lock)
            {
                Log.Clear();
            }
        }

        [Test]
        public async Task WhenAnAsyncPageTestIsRun_ThenTheRunnerReturnsAfterTheTestHasCompletelyFinished()
        {
            // Act
            await new Root().On<Ordered>().TestAsync();

            // Check
            CollectionAssert.Contains(Entries(), "Ordered.First:end");
            CollectionAssert.Contains(Entries(), "Ordered.Third:end");
        }

        [Test]
        public async Task WhenAsyncPageTestsAreRun_ThenTheyAreExecutedSequentiallyInTheOrderOfTheirLines()
        {
            // Act
            await new Root().On<Ordered>().TestAsync();

            // Check
            CollectionAssert.AreEqual(
                new[]
                {
                    "Ordered.BeforeFirstTestAsync",
                    "Ordered.First:start", "Ordered.First:end",
                    "Ordered.Second:start", "Ordered.Second:end",
                    "Ordered.Third:start", "Ordered.Third:end",
                    "Ordered.AfterLastTestAsync",
                },
                Entries());
        }

        [Test]
        public async Task WhenAnAsyncPageTestFails_ThenTheOriginalExceptionIsPropagatedAndTheStatisticIsMarkedAsFailed()
        {
            // Act
            var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await new Root().On<Failing>().TestAsync());

            // Check
            Assert.AreEqual("expected failure", exception.Message);
            var statistic = TestRunners.TestMethodStatistics.Single(e => e.MethodInfo.DeclaringType == typeof(FailingTests));
            Assert.IsFalse(statistic.Success);
            Assert.AreEqual(nameof(InvalidOperationException), statistic.Info);
            await Task.CompletedTask;
        }

        [Test]
        public async Task WhenAPageTestThrowsSynchronously_ThenTheOriginalExceptionIsPropagated()
        {
            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await new Root().On<FailingSynchronously>().TestAsync());

            // Check
            Assert.AreEqual("expected sync failure", exception.Message);
            await Task.CompletedTask;
        }

        [Test]
        public async Task WhenVoidPageTestsAreRunAsynchronously_ThenTheyAreExecuted()
        {
            // Act
            await new Root().On<Sync>().TestAsync();

            // Check
            CollectionAssert.AreEqual(new[] { "Sync.Run" }, Entries());
        }

        [Test]
        public async Task WhenTheBottomUpTestIsCalledAsynchronously_ThenTheChildTestsAreFinishedBeforeTheParentTestsStart()
        {
            // Act
            await new Root().On<Parent>().TestBottomUpAsync();

            // Check
            CollectionAssert.AreEqual(
                new[] { "Child:start", "Child:end", "Parent:start", "Parent:end" },
                Entries());
        }

        [Test]
        public async Task WhenAPageTestClassFilterIsGiven_ThenOnlyMatchingClassesAreExecutedAsynchronously()
        {
            // Act
            await new Root().On<Sync>().TestAsync(pageTestClassFilter: _ => false);

            // Check
            CollectionAssert.IsEmpty(Entries());
        }

        [Test]
        public async Task WhenAMethodFilterIsGiven_ThenOnlyMatchingMethodsAreExecutedAsynchronously()
        {
            // Act
            await new Root().On<Ordered>().TestAsync(methodFilter: method => method.Name == nameof(OrderedTests.Second));

            // Check
            CollectionAssert.AreEqual(
                new[] { "Ordered.BeforeFirstTestAsync", "Ordered.Second:start", "Ordered.Second:end", "Ordered.AfterLastTestAsync" },
                Entries());
        }

        [Test]
        public void WhenAnAsyncVoidPageTestIsRunAsynchronously_ThenANotSupportedExceptionIsThrown()
            => Assert.ThrowsAsync<NotSupportedException>(async () => await new Root().On<AsyncVoid>().TestAsync());

        [Test]
        public void WhenAnAsyncPageTestIsRunSynchronously_ThenANotSupportedExceptionIsThrown()
        {
            Assert.Throws<NotSupportedException>(() => new Root().On<Ordered>().Test());
            CollectionAssert.IsEmpty(Entries(), "No test must be started by the synchronous runner.");
        }

        [Test]
        public void WhenAnAsyncVoidPageTestIsRunSynchronously_ThenANotSupportedExceptionIsThrown()
            => Assert.Throws<NotSupportedException>(() => new Root().On<AsyncVoid>().Test());

        [Test]
        public void WhenAnAsyncLifecycleHookIsOverriddenAndTheTestsAreRunSynchronously_ThenANotSupportedExceptionIsThrown()
        {
            Assert.Throws<NotSupportedException>(() => new Root().On<Hooks>().Test());
            CollectionAssert.IsEmpty(Entries());
        }

        [Test]
        public void WhenOnlySynchronousPageTestsExist_ThenTheSynchronousRunnerStillWorks()
        {
            // Act
            new Root().On<Sync>().Test();

            // Check
            CollectionAssert.AreEqual(new[] { "Sync.Run" }, Entries());
        }

        private class Root : ProcessObject
        {
        }

        private class Ordered : ViewPageObject<object>, IChildOf<Root>
        {
        }

        private class Failing : ViewPageObject<object>, IChildOf<Root>
        {
        }

        private class FailingSynchronously : ViewPageObject<object>, IChildOf<Root>
        {
        }

        private class Sync : ViewPageObject<object>, IChildOf<Root>
        {
        }

        private class AsyncVoid : ViewPageObject<object>, IChildOf<Root>
        {
        }

        private class Hooks : ViewPageObject<object>, IChildOf<Root>
        {
        }

        private class Parent : ViewPageObject<object>, IChildOf<Root>
        {
        }

        private class Child : ViewPageObject<object>, IChildOf<Parent>
        {
        }

        private abstract class TestsBase<T> : PageObjectTests<T> where T : IPageObject
        {
            public override bool ReadyToRun
                => true;

            public override void BeforeFirstTest()
            {
            }
        }

        private class OrderedTests : TestsBase<Ordered>
        {
            public override async Task BeforeFirstTestAsync()
            {
                await Task.Yield();
                Add("Ordered.BeforeFirstTestAsync");
            }

            public override async Task AfterLastTestAsync()
            {
                await Task.Yield();
                Add("Ordered.AfterLastTestAsync");
            }

            [PageTest]
            public async Task First()
            {
                Add("Ordered.First:start");
                await Task.Delay(50);
                Add("Ordered.First:end");
            }

            [PageTest]
            public async Task<int> Second()
            {
                Add("Ordered.Second:start");
                await Task.Delay(10);
                Add("Ordered.Second:end");
                return 42;
            }

            [PageTest]
            public async Task Third()
            {
                Add("Ordered.Third:start");
                await Task.Delay(20);
                Add("Ordered.Third:end");
            }
        }

        private class FailingTests : TestsBase<Failing>
        {
            [PageTest]
            public async Task Fails()
            {
                await Task.Delay(10);
                throw new InvalidOperationException("expected failure");
            }
        }

        private class FailingSynchronouslyTests : TestsBase<FailingSynchronously>
        {
            [PageTest]
            public Task FailsBeforeTheFirstAwait()
                => throw new ArgumentException("expected sync failure");
        }

        private class SyncTests : TestsBase<Sync>
        {
            [PageTest]
            public void Run()
                => Add("Sync.Run");
        }

        private class AsyncVoidTests : TestsBase<AsyncVoid>
        {
            [PageTest]
            public async void Run()
            {
                await Task.Yield();
            }
        }

        private class HooksTests : TestsBase<Hooks>
        {
            public override Task BeforeFirstTestAsync()
                => Task.FromResult(0);

            [PageTest]
            public void Run()
                => Add("Hooks.Run");
        }

        private class ParentTests : TestsBase<Parent>
        {
            [PageTest]
            public async Task Run()
            {
                Add("Parent:start");
                await Task.Delay(10);
                Add("Parent:end");
            }
        }

        private class ChildTests : TestsBase<Child>
        {
            [PageTest]
            public async Task Run()
            {
                Add("Child:start");
                await Task.Delay(50);
                Add("Child:end");
            }
        }
    }
}
