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

namespace Trumpf.Coparoo.Desktop.Extensions
{
    using SmartBear.TestLeft.TestObjects;

    /// <summary>
    /// Extension methods for TestLeft controls.
    /// </summary>
    public static class IObjectExtensions
    {
        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="index">The index.</param>
        /// <returns>The item element.</returns>
        public static IObject ItemAt(this IObject node, int index)
        {
            return node.GetProperty<IObject>("Item", index);
        }

        /// <summary>
        /// Gets the typed item.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="index">The index.</param>
        /// <returns>The type item element.</returns>
        public static T ItemAt<T>(this IObject node, int index)
        {
            return node.GetProperty<T>("Item", index);
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The items.</returns>
        public static IObject Items(this IObject node)
        {
            return node.GetProperty<IObject>("Items");
        }
    }
}