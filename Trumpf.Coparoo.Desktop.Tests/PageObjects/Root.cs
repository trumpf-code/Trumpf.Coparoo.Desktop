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
    using Core;
    using NUnit.Framework;
    using Coparoo.Desktop;
    using Trumpf.Coparoo.Desktop.WinForms;
    using Trumpf.Coparoo.Desktop.Diagnostics;

    [TestFixture]
    public class Root
    {
        [Test]
        public void TheRootsRootIsTheRoot()
        {
            var a = new A();
            var r = (a as IUIObjectInternal).Root();
            Assert.AreEqual(a, r);
        }

        [Test]
        public void TheRootOfTheChildOfTheRootIsTheRoot()
        {
            var a = new A();
            var b = a.On<B>();
            Assert.AreEqual((a as IUIObjectInternal).Root(), (b as IUIObjectInternal).Root());
        }

        [Test]
        public void TheRootOfTheChildOfTheChildOfTheRootIsTheRoot()
        {
            var a = new A();
            var c = a.On<C>();
            Assert.AreEqual((a as IUIObjectInternal).Root(), (c as IUIObjectInternal).Root());
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
    }
}