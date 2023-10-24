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

    using Coparoo.Desktop;
    using NUnit.Framework;
    using WinForms;

    [TestFixture]
    public class On
    {
        private A P 
            => new A();

        [Test]
        public void FirstCandidate()
            => SetAndCheck(typeof(A));

        [Test]
        public void SecondCandidate()
            => SetAndCheck(typeof(B));

        private void SetAndCheck(Type expectedParentType)
        {
            C.T = expectedParentType;
            var visibleOnScreen = P.On<C>(e => e.VisibleOnScreen);
            Assert.AreEqual(expectedParentType, visibleOnScreen.Parent.GetType());
        }

        private class A : ProcessObject
        {
        }

        private class B : ViewPageObject<object>, IChildOf<A>
        {
        }

        private class C : ViewPageObject<object>, IChildOf<A>, IChildOf<B>
        {
            public static Type T { get; set; }

            protected override bool IsVisibleOnScreen => Parent.GetType().Equals(T);
        }
    }
}