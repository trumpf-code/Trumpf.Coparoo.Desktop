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

namespace Trumpf.Coparoo.Desktop.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SmartBear.TestLeft;
    using SmartBear.TestLeft.TestObjects;

    /// <summary>
    /// Extension methods for TestLeft search methods.
    /// </summary>
    internal static class IObjectTreeNodeExtensions
    {
        /// <summary>
        /// Finds all nodes with the given full class name.
        /// </summary>
        /// <param name="source">The node to search from.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="depth">The search depth.</param>
        /// <returns>The enumeration nodes.</returns>
        /// <typeparam name="T">The expected object type.</typeparam>
        internal static IReadOnlyList<T> MyFindAll<T>(this IObjectTreeNode source, ISearchPattern pattern, int depth = 1) where T : class, IObjectTreeNode
        {
            List<T> result = new List<T>();

            Queue<Tuple<IObjectTreeNode, int>> q = new Queue<Tuple<IObjectTreeNode, int>>();
            q.Enqueue(new Tuple<IObjectTreeNode, int>(source, 0));

            while (q.Count > 0)
            {
                Tuple<IObjectTreeNode, int> current = q.Dequeue();
                if (current == null)
                {
                    continue;
                }

                if (current.Item2 != 0)
                {
                    object value;
                    if (pattern.GetPatternItems().All((i) => current.Item1.TryGetProperty(i.Key, out value) && i.Value.Equals(value)))
                    {
                        result.Add(current.Item1.Cast<T>());
                        continue; // do not further search on this path
                    }
                }

                if (current.Item2 != depth)
                {
                    foreach (IObjectTreeNode child in current.Item1.Children)
                    {
                        q.Enqueue(new Tuple<IObjectTreeNode, int>(child, current.Item2 + 1));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Tries to return the value of the specified property.
        /// </summary>
        /// <typeparam name="T">The type of the object to read.</typeparam>
        /// <param name="source">The source node.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="result">The property value converted to a value of the T type, or default(T).</param>
        /// <returns>Whether the operation succeeded.</returns>
        internal static bool TryGetProperty<T>(this IObjectTreeNode source, string propertyName, out T result)
        {
            try
            {
                result = source.GetProperty<T>(propertyName);
                return true;
            }
            catch (InvocationException)
            {
                result = default(T);
                return false;
            }
        }
    }
}
