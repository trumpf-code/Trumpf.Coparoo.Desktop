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

    using Coparoo.Desktop;
    using Coparoo.Desktop.Core;
    using Exceptions;
    using NUnit.Framework;
    using WinForms;

    [TestFixture]
    public class PageObjectLocatorDynamic
    {
        private interface IB : IPageObject
        {
        }

        [Test]
        public void ClearTheRegister()
        {
            var r = new A() as IRootObjectInternal;
            var a = r.On<A>();
            var b = r.On<B>();
            Assert.AreEqual(2, r.PageObjectLocator.CacheObjectCount);
            r.PageObjectLocator.Clear();
            Assert.AreEqual(0, r.PageObjectLocator.CacheObjectCount);
        }

        [Test]
        public void RegisterThenUnregister()
        {
            var a = new A() as IRootObjectInternal;
            var r = a.PageObjectLocator;
            Assert.AreEqual(0, r.CacheObjectCount);
            var bbb = a.On<B>();
            Assert.AreEqual(1, r.CacheObjectCount);
            r.Unregister<B>();
            Assert.AreEqual(0, r.CacheObjectCount);
        }

        [Test]
        public void RegisterOtherType()
        {
            var a = new A() as IRootObjectInternal;
            var r = a.PageObjectLocator;

            a.PageObjectLocator.Unregister<B>();
            Assert.AreEqual(0, r.CacheObjectCount);
            var b = a.On<B>();
            Assert.AreEqual(typeof(B), b.GetType());
        }

        [Test]
        public void CheckRegisteredTypeCount()
        {
            var a = new A() as IRootObjectInternal;

            var ag = a.On<G>(e => e.Parent.GetType().Equals(typeof(A)));
            var bg = a.On<G>(e => e.Parent.GetType().Equals(typeof(B)));

            Assert.AreEqual(2, (a as IRootObjectInternal).PageObjectLocator.CacheObjectCount);

            Assert.AreEqual(typeof(A), ag.Parent.GetType());
            Assert.AreEqual(typeof(B), bg.Parent.GetType());
        }

        [Test]
        public void CheckRegisteredTypeCountForPageObjectWithTwoParents()
        {
            var a = new A() as IRootObjectInternal;

            var ag = a.On<G>(e => e.Parent.GetType().Equals(typeof(A)));
            var bg = a.On<G>(e => e.Parent.GetType().Equals(typeof(B)));

            Assert.AreEqual(typeof(A), ag.Parent.GetType());
            Assert.AreEqual(typeof(B), bg.Parent.GetType());
            Assert.AreEqual(2, a.PageObjectLocator.CacheObjectCount);

            var ag2 = a.On<G>(e => e.Parent.GetType().Equals(typeof(A)));
            var bg2 = a.On<G>(e => e.Parent.GetType().Equals(typeof(B)));

            Assert.AreEqual(typeof(A), ag2.Parent.GetType());
            Assert.AreEqual(typeof(B), bg2.Parent.GetType());

            Assert.AreEqual(2, a.PageObjectLocator.CacheObjectCount);
        }

        [Test]
        public void CheckTreeSize()
        {
            var r = new A();
            var tree = ((IPageObjectInternal)r).DotTree;
            var file = r.WritePdfTree();
            Assert.AreEqual(19, tree.EdgeCount);
            Assert.AreEqual(13, tree.NodeCount);
        }

        [Test]
        public void CheckValidOnCalls()
        {
            A r;
            A a;
            B b;
            C<object> co;
            C<int> ci;
            D<object> @do;
            D<int> di;

            r = new A();
            a = r.On<A>();
            b = r.On<B>();
            co = r.On<C<object>>();
            ci = r.On<C<int>>();
            @do = r.On<D<object>>();
            di = r.On<D<int>>();

            Assert.AreEqual(typeof(A), r.GetType());
            Assert.AreEqual(typeof(A), a.GetType());
            Assert.AreEqual(typeof(B), b.GetType());
            Assert.AreEqual(typeof(C<object>), co.GetType());
            Assert.AreEqual(typeof(C<int>), ci.GetType());
            Assert.AreEqual(typeof(D<object>), @do.GetType());
            Assert.AreEqual(typeof(D<int>), di.GetType());
        }

        [Test]
        public void CheckValidCacheReset()
        {
            var r = new A() as IRootObjectInternal;
            Assert.AreEqual(0, r.PageObjectLocator.CacheTypeCount);
            B b = r.On<B>();
            Assert.AreEqual(1, r.PageObjectLocator.CacheTypeCount);
            r = new A();
            Assert.AreEqual(0, r.PageObjectLocator.CacheTypeCount);
        }

        [Test]
        public void CheckPageObjectFinderCaching()
        {
            IRootObjectInternal r;
            A a;
            B b;
            C<object> co;
            C<int> ci;
            D<object> @do;
            D<int> di;

            r = new A() as IRootObjectInternal;
            Assert.AreEqual(0, r.PageObjectLocator.CacheTypeCount);
            a = r.On<A>();
            Assert.AreEqual(1, r.PageObjectLocator.CacheTypeCount);
            b = r.On<B>();
            Assert.AreEqual(2, r.PageObjectLocator.CacheTypeCount);
            co = r.On<C<object>>();
            Assert.AreEqual(3, r.PageObjectLocator.CacheTypeCount);
            ci = r.On<C<int>>();
            Assert.AreEqual(4, r.PageObjectLocator.CacheTypeCount);
            @do = r.On<D<object>>();
            Assert.AreEqual(5, r.PageObjectLocator.CacheTypeCount);
            di = r.On<D<int>>();
            Assert.AreEqual(6, r.PageObjectLocator.CacheTypeCount);

            a = r.On<A>();
            b = r.On<B>();
            co = r.On<C<object>>();
            ci = r.On<C<int>>();
            @do = r.On<D<object>>();
            di = r.On<D<int>>();

            Assert.AreEqual(6, r.PageObjectLocator.CacheTypeCount);
        }

        [Test]
        public void CheckInvalidOnCallWithOneTypeParameter()
        {
            // generic page object children of generic page objects are not supported
            Assert.Throws<PageObjectNotFoundException<E<object>>>(() => new A().On<E<object>>());
        }

        [Test]
        public void CheckInvalidOnCallWithTwoTypeParameters()
        {
            // generic page object children of generic page objects are not supported
            Assert.Throws<PageObjectNotFoundException<F<object, object>>>(() => new A().On<F<object, object>>());
        }

        [Test]
        public void ReRegister()
        {
            var a = new A();
            Assert.IsFalse(a.Register<B, A>());
            Assert.IsFalse(a.Register<C<object>, A>());
            Assert.IsTrue(a.Register<C<string>, A>());
        }

        [Test]
        public void CheckInvalidRegistering()
        {
            Assert.Throws<InvalidOperationException>(() => new A().Register<IB, A>());
        }

        [Test]
        public void CheckInvalidRegistering2()
        {
            Assert.Throws<InvalidOperationException>(() => new A().Register<A, IB>());
        }

        private class A : ProcessObject
        {
            public A()
            {
                Register<B, A>();
                Register<C<object>, A>();
                Register<C<int>, A>();
                Register<D<object>, A>();
                Register<D<int>, A>();
                Register<E<object>, D<object>>();
                Register<E<object>, D<int>>();
                Register<E<int>, D<object>>();
                Register<E<int>, D<int>>();
                Register<F<object, object>, D<object>>();
                Register<F<object, int>, D<object>>();
                Register<F<int, object>, D<object>>();
                Register<F<int, int>, D<object>>();
                Register<F<object, object>, D<int>>();
                Register<F<object, int>, D<int>>();
                Register<F<int, object>, D<int>>();
                Register<F<int, int>, D<int>>();
                Register<G, A>();
                Register<G, B>();
            }
        }

        private class B : ViewPageObject<object>, IB
        {
        }

        private class C<T> : ViewPageObject<object>, IChildOf<A>
        {
        }

        private class D<T> : ViewPageObject<object>
        {
        }

        private class E<T> : ViewPageObject<object>
        {
        }

        private class F<T, TT> : ViewPageObject<object>
        {
        }

        private class G : ViewPageObject<object>
        {
        }

        private class B2 : B
        {
        }
    }
}