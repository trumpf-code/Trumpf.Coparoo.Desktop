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
    using NUnit.Framework;
    using Trumpf.Coparoo.Desktop;
    using Trumpf.Coparoo.Desktop.Exceptions;
    using Trumpf.Coparoo.Desktop.WPF;

    [TestFixture]
    public class InterfaceResolution
    {
        [Test]
        public void WhenTryingToFindAControlObjectWithTwoImplementations_AnAmbiguousControlObjectMatchExceptionIsThrown()
            => Assert.Throws<AmbiguousControlObjectMatchException>(() => new Process().Find<IControlObjectWithTwoImplementationsYetNoCommonBaseClass>());

        [Test]
        public void WhenTryingToFindAControlObjectWithTwoImplementationsThatHaveAnAssignableBaseClass_ThenTheControlObjectIsResolvedToThatBaseClass()
        {
            var x = new Process().Find<IControlObjectWithTwoImplementationsAndACommonBaseClass>();
            Assert.AreEqual(typeof(Base), x.GetType());
        }

        private class Process : ProcessObject
        {
        }

        private class Page : PageObject, IChildOf<Process>
        {
            protected override Search SearchPattern => Search.Any;
        }

        #region A
        private interface IControlObjectWithTwoImplementationsYetNoCommonBaseClass : IControlObject
        {
        }

        private class FirstImplementation : ControlObject, IControlObjectWithTwoImplementationsYetNoCommonBaseClass
        {
            protected override Search SearchPattern => Search.Any;
        }

        private class SecondImplementation : ControlObject, IControlObjectWithTwoImplementationsYetNoCommonBaseClass
        {
            protected override Search SearchPattern => Search.Any;
        }
        #endregion

        #region B
        private interface IControlObjectWithTwoImplementationsAndACommonBaseClass : IControlObject
        {
        }

        private class Base : ControlObject, IControlObjectWithTwoImplementationsAndACommonBaseClass
        {
            protected override Search SearchPattern => Search.Any;
        }

        private class ThirdImplementation : Base
        {
            protected override Search SearchPattern => Search.Any;
        }

        private class ForthImplementation : Base
        {
            protected override Search SearchPattern => Search.Any;
        }
        #endregion
    }
}
