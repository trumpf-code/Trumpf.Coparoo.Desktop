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
    using System.Reflection;

    using Trumpf.Coparoo.Desktop.Logging.DotTree;

    /// <summary>
    /// Test class statistics class.
    /// </summary>
    internal class TestClassStatistic
    {
        private Dictionary<MethodInfo, TestMethodStatistics> testClassStatistics = new Dictionary<MethodInfo, TestMethodStatistics>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestClassStatistic"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public TestClassStatistic(Type type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets the test method statistics.
        /// </summary>
        public IEnumerable<TestMethodStatistic> TestMethodStatistics => testClassStatistics.SelectMany(e => e.Value.Values);

        /// <summary>
        /// Gets the type.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the tree representation of the statistics.
        /// </summary>
        internal DotTree DotTree
        {
            get
            {
                DotTree result = new DotTree(new Node { NodeType = Node.NodeTypes.PageTestClass, Id = Type.FullName, Caption = Type.Name + Environment.NewLine + Type.FullName, FrameColor = Node.Color.Grey });

                foreach (var testMethodStatistics in testClassStatistics.Values)
                {
                    Node.NodeTypes nodeType = testMethodStatistics.AnyUnimplemented ? Node.NodeTypes.PageTestIssue : Node.NodeTypes.PageTest;
                    Node.Color nodeColor = testMethodStatistics.AnyUnimplemented ? Node.Color.Orange : testMethodStatistics.AnyFailed ? Node.Color.Red : Node.Color.Green;

                    Node n = new Node { NodeType = nodeType, Id = Type.FullName + "." + testMethodStatistics.MethodInfo.Name, Caption = testMethodStatistics.ToString(), FrameColor = nodeColor };
                    result += n;
                    result += new Edge { Child = n.Id, Parent = result.Root.Id };
                }

                return result;
            }
        }

        /// <summary>
        /// Merge test class statistic.
        /// </summary>
        /// <param name="c1">Test class statistic to extended.</param>
        /// <param name="c2">Test class statistic to add.</param>
        /// <returns>The merged test class statistic.</returns>
        public static TestClassStatistic operator +(TestClassStatistic c1, TestClassStatistic c2)
        {
            Dictionary<MethodInfo, TestMethodStatistics> r = new Dictionary<MethodInfo, TestMethodStatistics>();

            foreach (var x in c1.testClassStatistics)
            {
                var matches = c2.testClassStatistics.Where(e => x.Key == e.Key).ToList();
                if (matches.Any())
                {
                    matches.ForEach(e => r.Add(x.Key, x.Value + e.Value));
                }
                else
                {
                    r.Add(x.Key, x.Value);
                }
            }

            return new TestClassStatistic(c1.Type) { testClassStatistics = r };
        }

        /// <summary>
        /// Add test method statistic.
        /// </summary>
        /// <param name="c1">Test class statistic to extended.</param>
        /// <param name="c2">Test method statistic to add.</param>
        /// <returns>The extended test class statistics.</returns>
        public static TestClassStatistic operator +(TestClassStatistic c1, TestMethodStatistic c2)
        {
            TestMethodStatistics value;

            if (c1.testClassStatistics.TryGetValue(c2.MethodInfo, out value))
            {
                c1.testClassStatistics[c2.MethodInfo] = value + c2;
            }
            else
            {
                c1.testClassStatistics.Add(c2.MethodInfo, new TestMethodStatistics(c2.MethodInfo) + c2);
            }

            return c1;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return Type.FullName + "\\n" + string.Join(Environment.NewLine, testClassStatistics.Values);
        }
    }
}