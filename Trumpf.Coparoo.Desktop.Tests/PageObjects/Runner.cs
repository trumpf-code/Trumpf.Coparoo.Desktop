// Copyright 2016, 2017, 2018 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
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
    using System.Linq.Expressions;

    using Coparoo.Desktop;
    using NUnit.Framework;
    using PageTests;
    using Trumpf.Coparoo.Desktop.Exceptions;
    using WinForms;

    [TestFixture]
    public class Runner
    {
        private A Root
        {
            get
            {
                var result = new A();
                result
                    .Configuration
                    .DependencyRegistrator
                    .Register<IDontKnow, DontKnow>();

                return result;
            }
        }

        [Test]
        public void WhenATestsClassHasUnregisteredTypes_ThenAnExceptionIsThrown()
            => Assert.Throws<TypeResolutionFailedException>(() => new A().On<B>().Test());

        [Test]
        public void WhenThePageTestsOfBAreExecuted_ThenTwoTestsExecuteInTotal()
        {
            // Prepare
            new BT(new DontKnow()).Reset();


            // Act
            Root.On<B>().Test();
            var testsExecuted = new BT(new DontKnow()).Counter;

            // Check
            Assert.AreEqual(2, testsExecuted);
        }

        [Test]
        public void WhenThePageTestsOfBAreExecuted_ThenOneTestExecutesInTotal()
        {
            // Prepare
            new CT().Reset();

            // Act
            Root.On<C>().Test();
            var testsExecuted = new CT().Counter;

            // Check
            Assert.AreEqual(1, new CT().Counter);
        }

        [Test]
        public void WhenTheBottomUpTestIsCalledOnTheRoot_ThenThreeTestsAreExecuted()
        {
            // Prepare
            new BT(new DontKnow()).Reset();
            new CT().Reset();

            // Act
            Root.TestBottomUp();
            var testsOfB = new BT(new DontKnow()).Counter;
            var testsOfC = new CT().Counter;

            // Check
            Assert.AreEqual(2, testsOfB);
            Assert.AreEqual(1, testsOfC);
        }

        [Test]
        public void WhenTheBottomUpTestIsCalledOnTheRootWithAFilter_ThenOnlyTheSelectedTestsAreExecuted()
        {
            // Prepare
            new BT(new DontKnow()).Reset();
            new CT().Reset();

            // Act
            Root.TestBottomUp(pageTestClassFilter: testClass => ((IFilterable)testClass).IncludeInTestrun);
            var testsOfB = new BT(new DontKnow()).Counter;
            var testsOfC = new CT().Counter;

            // Check
            Assert.AreEqual(0, testsOfB, $"Test should be skipped because of {nameof(IFilterable.IncludeInTestrun)} is false");
            Assert.AreEqual(1, testsOfC, $"Test should be execute because {nameof(IFilterable.IncludeInTestrun)} is true");
        }

        private class A : ProcessObject
        {
        }

        private class B : ViewPageObject<object>, IChildOf<A>
        {
        }

        private class C : ViewPageObject<object>, IChildOf<B>
        {
        }

        private interface IDontKnow
        {
        }

        private class DontKnow : IDontKnow
        {
        }

        private class BT : MyPageObjectTests<B>
        {
            public override bool IncludeInTestrun
                => false;

            private IDontKnow dontKnow;

            public BT(IDontKnow dontKnow)
                => this.dontKnow = dontKnow;

            [PageTest]
            public void B1()
            {
                Assert.IsNotNull(dontKnow);
                Assert.AreEqual(typeof(DontKnow), dontKnow.GetType());
                Increment();
            }

            [PageTest]
            public void B2()
                => Increment();
        }

        private class CT : MyPageObjectTests<C>
        {
            [PageTest]
            public void C1()
                => Increment();
        }

        private interface IFilterable
        {
            bool IncludeInTestrun { get; }
        }

        private abstract class MyPageObjectTests<T> : PageObjectTests<T>, IFilterable where T : IPageObject
        {
            private static int counter = 0;


            public int Counter
            {
                get { return counter; }
            }

            public override bool ReadyToRun
            {
                get { return true; }
            }

            public virtual bool IncludeInTestrun
                => true;

            protected void Increment()
                => counter++;

            public void Reset()
                => counter = 0;

            public override void BeforeFirstTest()
                => Expression.Empty();
        }
    }
}