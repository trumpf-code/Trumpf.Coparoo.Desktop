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

namespace Trumpf.Coparoo.Desktop.Waiting
{
    using System;

    /// <summary>
    /// Extensions for IAwait of type bool.
    /// </summary>
    public static class IAwaitExtensions
    {
        /// <summary>
        /// Wait for the expectation to become <c>true</c>.
        /// Throw on timeout.
        /// Use the default timeout as specified by configuration in the root object.
        /// </summary>
        /// <param name="awaitObject">The await object.</param>
        /// <param name="expectationText">The expectation text.</param>
        public static void WaitForTrue(this IAwait<bool> awaitObject, string expectationText)
        {
            Predicate<bool> expectation = v => v == true;
            awaitObject.WaitFor(expectation, expectationText);
        }

        /// <summary>
        /// Wait for the expectation to become <c>false</c>.
        /// Throw on timeout.
        /// Use the default timeout as specified by configuration in the root object.
        /// </summary>
        /// <param name="awaitObject">The await object.</param>
        /// <param name="expectationText">The expectation text.</param>
        public static void WaitForFalse(this IAwait<bool> awaitObject, string expectationText)
        {
            Predicate<bool> expectation = v => v == false;
            awaitObject.WaitFor(expectation, expectationText);
        }
    }
}