﻿// Copyright 2016 - 2023 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
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
    using System.Reflection;

    using Coparoo.Desktop;

    /// <summary>
    /// Internal page object interface.
    /// </summary>
    internal interface IPageObjectTestsInternal : IPageObjectTests
    {
        /// <summary>
        /// Initialize from a node.
        /// </summary>
        /// <param name="node">The node.</param>
        void Init(IPageObject node);

        /// <summary>
        /// Gets a value indicating whether the given page test method shall be executed.
        /// </summary>
        /// <param name="pageTest">The page test method info.</param>
        /// <returns>Whether the given page test method shall be executed.</returns>
        bool IsTestRunnable(MethodInfo pageTest);
    }
}