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

namespace Trumpf.Coparoo.Desktop.Tests.Loading
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using NUnit.Framework;

    /// <summary>
    /// Test class.
    /// Tests in this class must be executed separately as they need own app domains.
    /// </summary>
    [TestFixture, Ignore("needs external testleft assemblies")]
    public class LoaderTestss
    {
        private const string ddlFolder = "dlls";
        private readonly Action<string> logger = (line => Trace.WriteLine(line));

        [Test]
        public void WhenALocalDriverIsCreated_ThenAnExceptionIsThrown()
            => Assert.Throws<FileNotFoundException>(() => new SmartBear.TestLeft.LocalDriver());

        [Test]
        public void WhenALocalDriverIsCreatedInTheSameFunctionAsTheAssemblyRedirectionIsCalled_ThenAnExceptionIsThrown()
        {
            Assert.Throws<FileNotFoundException>(() =>
            {
                Loader.RegisterRedirections(logger, ddlFolder);
                var e = new SmartBear.TestLeft.LocalDriver();
            });
        }

        [Test]
        public void WhenALocalDriverIsCreatedInAnotherFunctionThanTheAssemblyRedirectionIsCalled_ThenNoExceptionIsThrown()
        {
            Loader.RegisterRedirections(logger, ddlFolder);
            CreateDriverInAnotherMethod();
        }

        private void CreateDriverInAnotherMethod()
            => new SmartBear.TestLeft.LocalDriver();
    }
}