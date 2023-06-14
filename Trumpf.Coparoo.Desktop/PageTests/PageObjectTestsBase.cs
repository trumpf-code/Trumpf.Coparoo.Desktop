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

namespace Trumpf.Coparoo.Desktop.PageTests
{
    using System;
    using System.Reflection;

    using Coparoo.Desktop;
    using Trumpf.Coparoo.Desktop.Core;
    using Trumpf.Coparoo.Desktop.Core.Waiting;
    using Trumpf.Coparoo.Desktop.Extensions;
    using Trumpf.Coparoo.Desktop.Waiting;

    /// <summary>
    /// Page tests base class.
    /// </summary>
    /// <typeparam name="TPageUnderTest">The page object under test.</typeparam>
    public abstract class PageObjectTests<TPageUnderTest> : IPageObjectTestClass<TPageUnderTest> where TPageUnderTest : IPageObject
    {
        /// <summary>
        /// Gets the configuration from the root object.
        /// </summary>
        private Lazy<Configuration> Configuration;

        /// <summary>
        /// Gets a value indicating whether the tests should be executed.
        /// </summary>
        public virtual bool Runnable 
            => true;

        /// <summary>
        /// Gets the page object for the page test.
        /// </summary>
        public TPageUnderTest Page { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the test is ready to run.
        /// </summary>
        public virtual bool ReadyToRun 
            => Page.VisibleOnScreen;
        /// <summary>
        /// Gets the page type.
        /// </summary>
        public Type PageType
            => typeof(TPageUnderTest);

        /// <summary>
        /// Function to execute before the first test.
        /// Default: If not ready to run, call goto.
        /// </summary>
        public virtual void BeforeFirstTest()
        {
            if (!ReadyToRun)
            {
                Page.Goto();
                Page.VisibleOnScreen.TryWaitFor(TimeSpan.FromSeconds(2));
            }
        }

        /// <summary>
        /// Function to execute after the last test.
        /// </summary>
        public virtual void AfterLastTest()
        {
        }

        /// <summary>
        /// Initialize the page object.
        /// </summary>
        /// <param name="pageUnderTest">The page object to drive the test.</param>
        void IPageObjectTestsInternal.Init(IPageObject pageUnderTest)
        {
            Configuration = new Lazy<Configuration>(() => Page.Root().Configuration);
            Page = (TPageUnderTest)pageUnderTest;
        }

        /// <summary>
        /// Gets a value indicating whether the given page test method shall be executed.
        /// </summary>
        /// <param name="pageTest">The page test method info.</param>
        /// <returns>Whether the given page test method shall be executed.</returns>
        bool IPageObjectTestsInternal.IsTestRunnable(MethodInfo pageTest)
            => IsTestRunnable(pageTest);

        /// <summary>
        /// Gets a value indicating whether the given page test method shall be executed.
        /// </summary>
        /// <param name="pageTest">The page test method info.</param>
        /// <returns>Whether the given page test method shall be executed.</returns>
        protected virtual bool IsTestRunnable(MethodInfo pageTest)
            => true;

        /// <summary>
        /// Gets an await-object.
        /// </summary>
        /// <typeparam name="T2">The underlying type.</typeparam>
        /// <param name="function">The function to wrap.</param>
        /// <returns>The wrapped object.</returns>
        protected IAwait<T2> Await<T2>(Func<T2> function)
        {
            var configuration = Configuration.Value;
            return new Await<T2>(function, "Element", GetType(), () => configuration.WaitTimeout, () => configuration.PositiveWaitTimeout, () => configuration.ShowWaitingDialog);
        }
    }
}