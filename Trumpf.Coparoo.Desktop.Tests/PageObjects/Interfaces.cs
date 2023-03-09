// Copyright 2016, 2017, 2018, 2019, 2020 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
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

    using NUnit.Framework;
    using Trumpf.Coparoo.Desktop.Diagnostics;
    using Trumpf.Coparoo.Desktop.WinForms;

    [TestFixture]
    public class Interfaces
    {
        private interface IA : IRootObject
        {
        }

        private interface IB : IPageObject
        {
        }

        private interface IC : IPageObject
        {
        }

        private interface ID : IControlObject
        {
        }

        private interface IE : ID
        {
        }

        private interface IF : IControlObject
        {
        }

        private interface IG<I> : IControlObject
            where I : IControlObject
        {
            I Item { get; }
        }

        private interface IH<I> : IControlObject
            where I : IControlObject
        {
            I Item { get; }
        }

        private interface IH<I, I2> : IControlObject
            where I : IControlObject
            where I2 : IControlObject
        {
        }

        [Test]
        public void WheAInterfaceTypeIsResolve_ThenTheReturnedTypeIsCorrect()
        {
            // Act
            var rootObject = ProcessObject.Resolve<IA>();
            var page = rootObject.On<IB>();
            var type = page.Resolve<IG<IF>>();

            // Check
            Assert.AreEqual(typeof(G<IF>), type);
        }

        [Test]
        public void WhenAGenericInterfaceIsSearchedFor_ThenToCorrenspondingGenericImplementationIsResolved()
        {
            // Act
            var rootObject = ProcessObject.Resolve<IA>();
            IG<IF> control = rootObject.Find<IG<IF>>();
            IF item = control.Item;

            // Check
            Assert.AreEqual(typeof(G<IF>), control.GetType());
            Assert.AreEqual(typeof(F), control.Item.GetType());
        }

        [Test]
        public void WhenTheResolveMethodIsCalledWithAnInterface_ThenTheCorrectRootObjectIsReturned()
        {
            // Act
            var rootObject = ProcessObject.Resolve<IA>();

            // Check
            Assert.AreEqual(typeof(A), rootObject.GetType());
            Assert.IsTrue(rootObject is IA);
        }

        [Test]
        public void WhenTheOnMethodIsCalledWithInterfaces_ThenThesInterfacesAreResolveToToCorrectType()
        {
            // Act
            var rootObject = ProcessObject.Resolve<IA>();
            var ia = rootObject.On<IA>();
            var ib = ia.On<IB>();
            var ic = ib.On<IC>();

            // Check
            Assert.IsTrue(ia is IA); // check IA from root
            Assert.AreEqual(typeof(A), ia.GetType());
            Assert.IsTrue(ib is IB); // check IB from IA
            Assert.AreEqual(typeof(B), ib.GetType());
            Assert.IsTrue(ic is IC); // check IC from IB
            Assert.AreEqual(typeof(C), ic.GetType());
        }

        [Test]
        public void WhenMupltipleControlObjectInterfacesMatch_ThenTheClosestMatchIsReturned()
        {
            // Act
            var rootObject = ProcessObject.Resolve<IA>();

            var d = rootObject.Find<ID>(); // IE and IF also implement ID, yet we want to get "the closest" implementation, hence D
            var e = rootObject.Find<IE>();
            var f = rootObject.Find<IF>();

            // Check
            Assert.AreEqual(typeof(D), d.GetType());
            Assert.AreEqual(typeof(E), e.GetType());
            Assert.AreEqual(typeof(F), f.GetType());
        }

        private class A : ProcessObject, IA
        {
        }

        private class B : ViewPageObject<object>, IB, IChildOf<IA>
        {
        }

        private class C : ViewPageObject<object>, IC, IChildOf<IB>
        {
        }

        private class D : ViewControlObject<object>, ID
        {
        }

        private class E : D, IE
        {
        }

        private class F : ViewControlObject<object>, IF
        {
        }

        private class G<I> : ViewControlObject<object>, IG<I>
            where I : IControlObject
        {
            public I Item
                => Find<I>();

            internal void Init(F f)
                => throw new NotImplementedException();
        }

        private class H<I> : ViewControlObject<object>, IH<I>
            where I : IControlObject
        {
            public I Item
                => Find<I>();

            internal void Init(F f)
                => throw new NotImplementedException();
        }
    }
}