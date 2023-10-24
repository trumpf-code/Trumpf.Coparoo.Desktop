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
    using NUnit.Framework;
    using System.IO;
    using System.Reflection;
    using Trumpf.Coparoo.Desktop.Exceptions;
    using Trumpf.Coparoo.Desktop.WPF;

    public class A : ProcessObject
    {
    }

    public class B : ViewPageObject<object>, IChildOf<A>
    {
    }

    [TestFixture]
    public class MultipleLoadTests
    {
        private const string OTHER_DLL_FILENAME = "copy.dll";

        [Test]
        public void WhenTheSamePageObjectIsLoadedTwiceFromDifferentLocations_ThenAccessingThatTypeViaOnThrowsAnException()
        {
            // Prepare
            var currentAssemblyLocation = GetType().Assembly.Location;

            // delte copy if it exists
            if (File.Exists(OTHER_DLL_FILENAME))
                File.Delete(OTHER_DLL_FILENAME);
            File.Copy(currentAssemblyLocation, OTHER_DLL_FILENAME);

            // load the currently running assembly again
            Assembly.LoadFrom(OTHER_DLL_FILENAME);

            // Act & Check
            Assert.Throws<AmbiguousParentObjectFoundException>(() => new A().On<B>());
        }
    }
}