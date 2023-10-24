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

namespace Trumpf.Coparoo.Desktop.PageTests.Statistics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Logging.DotTree;

    /// <summary>
    /// Page objects statistics class.
    /// </summary>
    internal class PageObjectStatistic
    {
        /// <summary>
        /// Test class type -> test class statistics.
        /// </summary>
        private Dictionary<Type, TestClassStatistic> pageObjectStatistics = new Dictionary<Type, TestClassStatistic>();

        /// <summary>
        /// Gets the test method statistics.
        /// </summary>
        public IEnumerable<TestMethodStatistic> TestMethodStatistics => pageObjectStatistics.Values.SelectMany(e => e.TestMethodStatistics);

        /// <summary>
        /// Gets the tree representation of the statistics.
        /// </summary>
        internal List<DotTree> DotTrees
        {
            get
            {
                var result = new List<DotTree>();
                foreach (var pageObjectStatistic in pageObjectStatistics.Values)
                {
                    result.Add(pageObjectStatistic.DotTree);
                }

                return result;
            }
        }

        /// <summary>
        /// Merge in test class statistics.
        /// </summary>
        /// <param name="c1">The page object statistics to extend.</param>
        /// <param name="c2">The test class statistics to add.</param>
        /// <returns>The extended page object statistic.</returns>
        public static PageObjectStatistic operator +(PageObjectStatistic c1, TestClassStatistic c2)
        {
            TestClassStatistic value;

            if (c1.pageObjectStatistics.TryGetValue(c2.Type, out value))
            {
                c1.pageObjectStatistics[c2.Type] = value + c2;
            }
            else
            {
                c1.pageObjectStatistics.Add(c2.Type, c2);
            }

            return c1;
        }
    }
}