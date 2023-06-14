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
    using Coparoo.Desktop;
    using NUnit.Framework;
    using Trumpf.Coparoo.Desktop.Core;
    using Trumpf.Coparoo.Desktop.Extensions;
    using WinForms;

    /// <summary>
    /// Test class.
    /// </summary>
    [TestFixture]
    public class FindTests
    {
        private const int DEFAULTCONTROLSEARCHDEPTH = 99;
        private const int DEFAULTPAGEOBJECTSEARCHDEPTH = 98;
        private const int OTHERCONTROLSEARCHDEPTH = 97;

        private A P => new A();

        private A Q
        {
            get
            {
                var result = new A();
                result.Configuration.PageObjectSearchDepth = DEFAULTPAGEOBJECTSEARCHDEPTH;
                result.Configuration.ControlSearchDepth = DEFAULTCONTROLSEARCHDEPTH;
                return result;
            }
        }

        [Test]
        public void InitialProcessObjectSearchDephts()
        {
            Assert.AreEqual(P.Configuration.ControlSearchDepth, (P as IUIObjectInternal).ControlSearchDepth);
            Assert.AreEqual(P.Configuration.PageObjectSearchDepth, (P as IUIObjectInternal).PageObjectSearchDepth);
        }

        [Test]
        public void InitialPageObjectSearchDephts()
        {
            Assert.AreEqual(P.Configuration.ControlSearchDepth, (P.On<A>() as IUIObjectInternal).ControlSearchDepth);
            Assert.AreEqual(P.Configuration.PageObjectSearchDepth, (P.On<A>() as IUIObjectInternal).PageObjectSearchDepth);
        }

        [Test]
        public void InitialControlObjectSearchDephts()
        {
            var co = P.Find<CO>() as IUIObjectInternal;
            Assert.AreEqual(P.Configuration.ControlSearchDepth, co.ControlSearchDepth);
            Assert.AreEqual(P.Configuration.ControlSearchDepth, co.PageObjectSearchDepth);
        }

        [Test]
        public void CustomProcessObjectSearchDephts()
        {
            Assert.AreEqual(Q.Configuration.ControlSearchDepth, (Q as IUIObjectInternal).ControlSearchDepth);
            Assert.AreEqual(Q.Configuration.PageObjectSearchDepth, (Q as IUIObjectInternal).PageObjectSearchDepth);
        }

        [Test]
        public void CustomPageObjectSearchDephts()
        {
            Assert.AreEqual(Q.Configuration.ControlSearchDepth, (Q.On<A>() as IUIObjectInternal).ControlSearchDepth);
            Assert.AreEqual(Q.Configuration.PageObjectSearchDepth, (Q.On<A>() as IUIObjectInternal).PageObjectSearchDepth);
        }

        [Test]
        public void CustomControlObjectSearchDephts()
        {
            var co = Q.Find<CO>() as IUIObjectInternal;
            Assert.AreEqual(Q.Configuration.ControlSearchDepth, co.ControlSearchDepth);
            Assert.AreEqual(Q.Configuration.ControlSearchDepth, co.PageObjectSearchDepth);
        }

        [Test]
        public void CustomControlObjectSearchDephtsInFind()
        {
            Assert.AreEqual(OTHERCONTROLSEARCHDEPTH, (Q.Find<CO>(null, null, OTHERCONTROLSEARCHDEPTH) as IUIObjectInternal).PageObjectSearchDepth);
            Assert.AreEqual(OTHERCONTROLSEARCHDEPTH + 10, (Q.Find<CO>(null, null, OTHERCONTROLSEARCHDEPTH + 10) as IUIObjectInternal).PageObjectSearchDepth);
        }

        private class CO : ViewControlObject<object>
        {
        }

        private class CP : ViewControlObject<object>
        {
        }

        private class A : ProcessObject
        {
            public A()
            {
                Configuration.ControlSearchDepth = DEFAULTCONTROLSEARCHDEPTH;
                Configuration.PageObjectSearchDepth = DEFAULTPAGEOBJECTSEARCHDEPTH;
            }
        }

        private class B : ViewPageObject<object>, IChildOf<A>
        {
        }
    }
}